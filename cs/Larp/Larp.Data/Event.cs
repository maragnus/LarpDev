namespace Larp.Data;

[PublicAPI]
public enum EventRsvp
{
    Unanswered,
    No, // Will not attend
    Maybe, // Potentially attending
    Yes, // Intention to attend
    Confirmed, // User confirmed their attendance (post letter)
    Approved, // Admin has approved user's attendance
}

[PublicAPI]
public class EventComponent
{
    public string ComponentId { get; set; } = default!;
    public string? Name { get; set; }
    public DateOnly Date { get; set; }
}

[PublicAPI]
public class Event
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = default!;

    public string GameId { get; set; } = default!;
    public string? Title { get; set; }
    public string? Location { get; set; }
    public DateOnly Date { get; set; }
    public string? EventType { get; set; }
    public bool CanRsvp { get; set; }
    public bool IsHidden { get; set; }
    public string? ImportId { get; set; }
    public EventComponent[] Components { get; set; } = Array.Empty<EventComponent>();
    public string? LetterTemplateId { get; set; }
}

[PublicAPI]
public enum EventAttendanceType
{
    Player,
    Staff,
    Mixed
}

[PublicAPI]
public class EventAttendance : Attendance
{
    public EventAttendance()
    {
    }

    public EventAttendance(Attendance attendance, Event @event)
    {
        Id = attendance.Id;
        AccountId = attendance.AccountId;
        MwFifth = attendance.MwFifth;
        EventId = attendance.EventId;
        Event = @event;
    }

    public Event Event { get; set; } = default!;
}

[PublicAPI]
public class Attendance
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public MwFifthAttendance? MwFifth { get; set; }
}

public class MwFifthAttendance
{
    public int? Moonstone { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string[] CharacterIds { get; set; } = Array.Empty<string>();
}