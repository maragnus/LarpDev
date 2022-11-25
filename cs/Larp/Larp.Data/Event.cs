using Larp.Protos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Larp.Data;

public class EventComponent
{
    public EventComponent()
    {
    }

    public EventComponent(Protos.EventComponent ec)
    {
        Name = ec.Name;
        Date = DateTimeOffset.Parse(ec.Date);
    }

    public string ComponentId { get; set; } = null!;
    public string? Name { get; set; }
    public DateTimeOffset Date { get; set; }

    public Protos.EventComponent ToProto()
    {
        return new Protos.EventComponent()
        {
            Name = Name,
            Date = Date.ToString("O")
        };
    }
}

public class Event
{
    public Event()
    {
    }

    public Event(Protos.Event e)
    {
        Id = e.EventId;
        Components.AddRange(e.Components.Select(x => new EventComponent(x)));
        Date = DateTimeOffset.Parse(e.Date);
        Location = e.Location;
        Title = e.Title;
        CanRsvp = e.Rsvp;
        EventType = e.EventType;
        GameId = e.GameId;
        IsHidden = e.Hidden;
    }

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

    public Protos.Event ToProto()
    {
        var ev = new Protos.Event()
        {
            Date = Date.ToString("O"),
            Hidden = IsHidden,
            Location = Location,
            Rsvp = CanRsvp,
            Title = Title,
            EventId = Id,
            EventType = EventType,
            GameId = GameId
        };
        ev.Components
            .AddRange(Components.Select(x => x.ToProto()));
        return ev;
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