using System.Text;
using System.Web;

namespace Larp.Common;

public record CalendarEvent(
    DateOnly StartDate,
    TimeOnly? StartTime,
    DateOnly? EndDate,
    TimeOnly? EndTime,
    string Title,
    string? Location,
    string? Description);
    
public static class CalendarGenerator
{
    // https://www.rfc-editor.org/rfc/rfc5545.html
    public static MemoryStream CreateIcsStream(IEnumerable<CalendarEvent> events)
    {
        // Set the format for dates in the .ics file
        const string dateTimeFormat = "yyyyMMddTHHmmssZ";
        const string dateFormat = "yyyyMMdd";

        // Create the contents of the .ics file
        var sb = new StringBuilder();
        AppendLine(sb, "BEGIN", "VCALENDAR");
        AppendLine(sb, "VERSION", "2.0");
        foreach (var ev in events)
        {
            var start = ev.StartTime.HasValue
                ? ev.StartDate.ToDateTime(ev.StartTime.Value).ToUniversalTime().ToString(dateTimeFormat)
                : ev.StartDate.ToString(dateFormat);
            var end = ev.EndDate.HasValue
                ? ev.EndTime.HasValue
                    ? ev.EndDate.Value.ToDateTime(ev.EndTime.Value).ToUniversalTime().ToString(dateTimeFormat)
                    : ev.EndDate.Value.ToString(dateFormat)
                : ev.StartDate.AddDays(1).ToString(dateFormat);
            
            AppendLine(sb, "BEGIN", "VEVENT");
            AppendLine(sb, "DTSTART", start);
            AppendLine(sb, "DTEND", end);
            AppendLine(sb, "SUMMARY", IcsEscapeString(ev.Title));
            AppendLine(sb, "LOCATION", IcsEscapeString(ev.Location));
            AppendLine(sb, "DESCRIPTION", IcsEscapeString(ev.Description));
            AppendLine(sb, "END", "VEVENT");
        }
        AppendLine(sb, "END", "VCALENDAR");

        // Convert the string builder to a memory stream
        return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    private static void AppendLine(StringBuilder sb, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            return;
        
        var line = $"{key}:{value}";
        if (line.Length <= 75)
        {
            sb.Append(line).Append("\r\n");
            return;
        }
        
        sb.Append(line[..75]).Append("\r\n");
        foreach (var block in line.Skip(75).Chunk(74))
            sb.Append(' ').Append(block).Append("\r\n");
    }
    
    // Escapes characters that are not allowed in text fields
    private static string? IcsEscapeString(string? input) => input?
        .Replace("\\", "\\\\")
        .ReplaceLineEndings("\\n")
        .Replace(";", @"\;")
        .Replace(",", @"\,");

    public static string CreateGoogleCalendarUrl(CalendarEvent ev)
    {
        const string dateTimeFormat = "yyyyMMddTHHmmssZ";
        const string dateFormat = "yyyyMMdd";

        // Base URL for creating a Google Calendar event
        string baseUrl = "https://calendar.google.com/calendar/render?action=TEMPLATE";

        var start = ev.StartTime.HasValue
            ? ev.StartDate.ToDateTime(ev.StartTime.Value).ToUniversalTime().ToString(dateTimeFormat)
            : ev.StartDate.ToString(dateFormat);
        var end = ev.EndDate.HasValue
            ? ev.EndTime.HasValue
                ? ev.StartDate.ToDateTime(ev.EndTime.Value).ToUniversalTime().ToString(dateTimeFormat)
                : ev.EndDate.Value.ToString(dateFormat)
            : ev.StartDate.AddDays(1).ToString(dateFormat);

        // Construct the URL
        return $"{baseUrl}&text={HttpUtility.UrlEncode(ev.Title)}" +
                  $"&dates={start}/{end}" +
                  $"&location={HttpUtility.UrlEncode(ev.Location)}" +
                  $"&details={HttpUtility.UrlEncode(ev.Description)}" +
                  "&sf=true&output=xml";
    }
}