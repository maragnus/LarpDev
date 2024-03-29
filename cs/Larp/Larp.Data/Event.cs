﻿using System.Text.Json.Serialization;
using Larp.Data.MwFifth;

namespace Larp.Data;

public enum EventList
{
    All,
    Upcoming,
    Past,
    Dashboard
}

public static class LetterNames
{
    public const string PreEvent = nameof(PreEvent);
    public const string PostEvent = nameof(PostEvent);
    public const string BetweenEvent = nameof(BetweenEvent);
}

[PublicAPI]
public class EventComponent
{
    public string ComponentId { get; set; } = default!;
    public string? Name { get; set; }
    public DateOnly Date { get; set; }
    public bool Free { get; set; }
}

public class EventAndLetters
{
    public Event Event { get; set; } = default!;
    public Letter[] Letters { get; set; } = default!;
}

public class EventLetter
{
    public string Name { get; set; } = default!;
    public bool IsOpen { get; set; }
    public string LetterTemplateId { get; set; } = default!;
}

[PublicAPI]
public class Event
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string GameId { get; set; } = default!;

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Location { get; set; }

    public DateOnly Date { get; set; }
    
    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Summary { get; set; }
    
    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? EventType { get; set; }

    public bool IsHidden { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ImportId { get; set; }

    public EventComponent[] Components { get; set; } = Array.Empty<EventComponent>();

    public EventLetter[] LetterTemplates { get; set; } = Array.Empty<EventLetter>();

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PreregistrationNotes { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AdminNotes { get; set; }

    public int EventCost { get; set; }

    public int ChronicleCost { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Chapter { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Clarifies { get; set; }

    public int? Attendees { get; set; }

    private static bool Summarize<T>(IEnumerable<T> oldList, IEnumerable<T> newList, Func<T, string> transformer,
        out string[] oldItems, out string[] newItems)
    {
        oldItems = oldList.Select(transformer).OrderBy(x => x).ToArray();
        newItems = newList.Select(transformer).OrderBy(x => x).ToArray();
        return !oldItems.SequenceEqual(newItems);
    }

    public static Dictionary<string, ChangeSummary> BuildChangeSummary(Event? oldEvent, Event? newEvent)
    {
        var result = new Dictionary<string, ChangeSummary>();
        if (oldEvent == null || newEvent == null)
            return result;

        foreach (var property in typeof(Event).GetProperties())
        {
            var oldValue = property.GetValue(oldEvent);
            var newValue = property.GetValue(newEvent);

            if (newValue is string[] newStrings && oldValue is string[] oldStrings)
            {
                if (Summarize(oldStrings, newStrings, x => x, out var oldItems, out var newItems))
                    result.Add(property.Name, new ChangeSummary(oldItems, newItems));
            }
            else if (newValue is EventComponent[] newComponents && oldValue is EventComponent[] oldComponents)
            {
                if (Summarize(oldComponents, newComponents, x => x.Name ?? "Unnamed Component", out var oldItems,
                        out var newItems))
                    result.Add(property.Name, new ChangeSummary(oldItems, newItems));
            }
            else if (newValue is EventLetter[] newLetters && oldValue is EventLetter[] oldLetters)
            {
                if (Summarize(oldLetters, newLetters, x => $"{x.Name} ({(x.IsOpen ? "Open" : "Closed")})",
                        out var oldItems, out var newItems))
                    result.Add(property.Name, new ChangeSummary(oldItems, newItems));
            }
            else if (((oldValue == null) != (newValue == null)) || oldValue?.Equals(newValue) == false)
            {
                result.Add(property.Name, new ChangeSummary(oldValue?.ToString(), newValue?.ToString()));
            }
        }

        return result;
    }
}