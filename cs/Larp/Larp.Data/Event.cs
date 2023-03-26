﻿namespace Larp.Data;

[PublicAPI]
public enum EventRsvp {
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
    public string ComponentId { get; set; } = null!;
    public string? Name { get; set; }
    public DateTimeOffset Date { get; set; }
}

[PublicAPI]
public class Event
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string GameId { get; set; } = null!;
    public string? Title { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? EventType { get; set; }
    public bool CanRsvp { get; set; }
    public bool IsHidden { get; set; }
    public List<EventComponent> Components { get; set; } = new();
}

[PublicAPI]
public enum EventAttendanceType
{
    Player,
    Staff,
    Mixed
}

[PublicAPI]
public class ComponentAttendance
{
    public string ComponentId { get; set; } = null!;
    public string CharacterId { get; set; } = null!;
    public EventAttendanceType Type { get; set; }
}

[PublicAPI]
public class Attendance
{
    public string EventId { get; set; } = null!;
    public string AccountId { get; set; } = null!;
    public List<ComponentAttendance> ComponentAttendances { get; set; } = new();
}