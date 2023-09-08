namespace Larp.Data;

public class EventAttendanceList
{
    public string? AccountId { get; set; }
    public EventAttendanceItem[] Events { get; set; } = Array.Empty<EventAttendanceItem>();
    public Dictionary<string, LetterTemplate> LetterTemplates { get; set; } = new();
}

public class EventAttendanceItem
{
    public Event Event { get; set; } = default!;
    public Attendance? Attendance { get; set; }
    public Letter? PreEvent { get; set; }
    public Letter? PostEvent { get; set; }
    public Letter? BetweenEvent { get; set; }
}