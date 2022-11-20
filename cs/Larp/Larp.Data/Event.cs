using Larp.Protos;

namespace Larp.Data;

public class EventComponent
{
    public string ComponentId { get; set; } = null!;
    public string? Name { get; set; }
    public DateTimeOffset Date { get; set; }
}

public class Event
{
    public string EventId { get; set; } = null!;
    public string GameId { get; set; } = null!;
    public string? Title { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? EventType { get; set; }
    public bool CanRsvp { get; set; }
    public bool IsHidden { get; set; }
    public List<EventComponent> Components { get; set; } = new();

    public Protos.Event ToProto()
    {
        throw new NotImplementedException();
    }
}

public class ComponentAttendance
{
    public string ComponentId { get; set; } = null!;
    public string CharacterId { get; set; } = null!;
    public EventAttendanceType Type { get; set; }
}

public class Attendance
{
    public string EventId { get; set; } = null!;
    public string AccountId { get; set; } = null!;
    public List<ComponentAttendance> ComponentAttendances { get; set; } = new();
}