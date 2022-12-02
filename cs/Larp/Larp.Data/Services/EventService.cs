using Larp.Protos;
using Microsoft.Extensions.Internal;
using MongoDB.Driver;

namespace Larp.Data.Services;

public interface IEventService
{
    Task<IEnumerable<Protos.Event>> ListEventsForAccount(string accountId, bool includePast, bool includeFuture,
        bool includeAttendance);

    Task<Protos.Event> Rsvp(string accountId, string eventId, EventRsvp rsvp);
    Task<Protos.Event> GetEvent(string eventId);
}

public class EventService : IEventService
{
    private readonly ISystemClock _clock;
    private readonly LarpContext _larpContext;

    public EventService(LarpContext larpContext, ISystemClock clock)
    {
        _larpContext = larpContext;
        _clock = clock;
    }

    public async Task<IEnumerable<Protos.Event>> ListEventsForAccount(string accountId, bool includePast,
        bool includeFuture,
        bool includeAttendance)
    {
        var now = _clock.UtcNow;
        var events = await _larpContext.Events.Find(_ => true)
            .Sort(Builders<Data.Event>.Sort.Ascending(x => x.Date))
            .ToListAsync();

        var filteredEvents =
            from ev in events
            let isPast = ev.Date <= now
            where (!isPast || includePast)
            where (isPast || includeFuture)
            select ev;

        return events.Select(e => e.ToProto());
    }

    public Task<Protos.Event> Rsvp(string accountId, string eventId, EventRsvp rsvp)
    {
        throw new NotImplementedException();
    }

    public async Task<Protos.Event> GetEvent(string eventId)
    {
        var e = await _larpContext.Events
            .Find(x => x.Id == eventId)
            .FirstOrDefaultAsync();
        return e.ToProto();
    }
}