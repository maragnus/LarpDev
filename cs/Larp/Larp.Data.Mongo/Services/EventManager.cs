using Larp.Common;
using Larp.Common.Exceptions;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Larp.Data.Mongo.Services;

public class EventManager
{
    private readonly LarpContext _larpContext;
    private readonly MwFifthCharacterManager _characterManager;
    private readonly ILogger<EventManager> _logger;

    public EventManager(LarpContext larpContext, MwFifthCharacterManager characterManager, ILogger<EventManager> logger)
    {
        _larpContext = larpContext;
        _characterManager = characterManager;
        _logger = logger;
    }

    public async Task<EventAndLetters[]> GetEvents()
    {
        var letters = (await _larpContext.Letters
                .Find(x => x.State == LetterState.Approved || x.State == LetterState.Submitted)
                .Project(x => new Letter() { EventId = x.EventId, State = x.State })
                .ToListAsync())
            .ToLookup(x => x.EventId);

        var events = await _larpContext.Events.Find(_ => true)
            .SortByDescending(x => x.Date)
            .ToListAsync();
        return events.Select(x => new EventAndLetters
        {
            Event = x,
            Letters = letters[x.EventId].ToArray()
        }).ToArray();
    }

    public async Task<EventsAndLetters> GetEvents(string? accountId, EventList list)
    {
        var now = DateOnly.FromDateTime(DateTime.Today);

        var letters = accountId == null
            ? new Dictionary<string, Letter>()
            : (await _larpContext.Letters.Find(x => x.AccountId == accountId)
                .Project(x => new Letter()
                    { LetterId = x.LetterId, Name = x.Name, EventId = x.EventId, State = x.State })
                .ToListAsync())
            .ToDictionary(x => x.LetterId);

        var filter = list switch
        {
            EventList.Past => Builders<Event>.Filter.Where(x => x.Date <= now.AddDays(4) && !x.IsHidden),
            EventList.Upcoming => Builders<Event>.Filter.Where(x => x.Date >= now.AddDays(-4) && !x.IsHidden),
            _ => throw new ArgumentOutOfRangeException(nameof(list), list, null)
        };

        var events = await _larpContext.Events.Find(filter).ToListAsync();
        var templateIds = letters.Values.Select(x => x.TemplateId).Distinct().ToList();
        var templates = await _larpContext.LetterTemplates
            .Find(template => templateIds.Contains(template.LetterTemplateId)).ToListAsync();

        return new EventsAndLetters()
        {
            Events = events.ToDictionary(x => x.EventId),
            Accounts = await GetAccountNames(),
            Letters = letters,
            LetterTemplates = templates.ToDictionary(x => x.LetterTemplateId)
        };
    }

    private async Task<Dictionary<string, AccountName>> GetAccountNames()
    {
        var names = await _larpContext.Accounts.Find(_ => true)
            .Project(account => new AccountName()
            {
                AccountId = account.AccountId,
                Name = account.Name
            })
            .ToListAsync();
        return names.ToDictionary(x => x.AccountId);
    }

    public async Task<Event> GetEvent(string eventId)
    {
        return await _larpContext.Events.FindOneAsync(x => x.EventId == eventId)
               ?? throw new ResourceNotFoundException();
    }

    public async Task SaveEvent(string eventId, Event @event)
    {
        await _larpContext.Events.UpdateOneAsync(x => x.EventId == eventId,
            Builders<Event>.Update
                .Set(x => x.GameId, @event.GameId)
                .Set(x => x.Title, @event.Title)
                .Set(x => x.EventType, @event.EventType)
                .Set(x => x.Location, @event.Location)
                .Set(x => x.Date, @event.Date)
                .Set(x => x.IsHidden, @event.IsHidden)
                .Set(x => x.LetterTemplates, @event.LetterTemplates)
                .Set(x => x.Components, @event.Components)
        );
    }

    public async Task DeleteEvent(string eventId)
    {
        var attendees = await _larpContext.Attendances.CountDocumentsAsync(x => x.EventId == eventId);
        if (attendees > 0)
            throw new BadRequestException($"Cannot delete event, it has {attendees} attendees");
        await _larpContext.Events.DeleteOneAsync(x => x.EventId == eventId);
    }

    public async Task SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone,
        string[] characterIds)
    {
        if (!attended)
        {
            await _larpContext.Attendances.DeleteOneAsync(x => x.EventId == eventId && x.AccountId == accountId);
            return;
        }

        var update = Builders<Attendance>.Update
            .Set(x => x.EventId, eventId)
            .Set(x => x.AccountId, accountId)
            .Set(x => x.MwFifth,
                new MwFifthAttendance
                {
                    Moonstone = moonstone,
                    CharacterIds = characterIds
                });
        var upsert = new UpdateOptions() { IsUpsert = true };

        await _larpContext.Attendances.UpdateOneAsync(
            attendance => attendance.EventId == eventId && attendance.AccountId == accountId,
            update,
            upsert);

        await _characterManager.UpdateMoonstone(accountId);
    }

    public async Task<Attendance[]> GetEventAttendances(string eventId) =>
        (await _larpContext.Attendances
            .Find(attendance => attendance.EventId == eventId)
            .ToListAsync())
        .ToArray();

    public async Task<StringResult> DraftEvent()
    {
        var id = ObjectId.GenerateNewId().ToString();
        await _larpContext.Events.InsertOneAsync(new Event()
        {
            EventId = id,
            Title = "New Event",
            Date = DateOnly.FromDateTime(DateTime.Today),
            EventType = "Game",
            GameId = await _larpContext.Games.Find(x => x.Name == GameState.GameName).Project(x => x.Id)
                         .FirstOrDefaultAsync()
                     ?? throw new BadRequestException("GameId could not be found"),
            IsHidden = true
        });
        return StringResult.Success(id);
    }

    public async Task<EventsAndLetters> GetEvents(EventList list, string? accountId)
    {
        var now = DateOnly.FromDateTime(DateTime.Today);

        var letters = accountId == null
            ? new Dictionary<string, Letter>()
            : (await _larpContext.Letters.Find(x => x.AccountId == accountId)
                .Project(x => new Letter()
                    { LetterId = x.LetterId, Name = x.Name, EventId = x.EventId, State = x.State })
                .ToListAsync())
            .ToDictionary(x => x.LetterId);

        var filter = list switch
        {
            EventList.Past => Builders<Event>.Filter.Where(x => x.Date <= now.AddDays(4) && !x.IsHidden),
            EventList.Upcoming => Builders<Event>.Filter.Where(x => x.Date >= now.AddDays(-4) && !x.IsHidden),
            _ => throw new ArgumentOutOfRangeException(nameof(list), list, null)
        };

        var events = await _larpContext.Events.Find(filter).ToListAsync();
        var templateIds = letters.Values.Select(x => x.TemplateId).Distinct().ToList();
        var templates = await _larpContext.LetterTemplates
            .Find(template => templateIds.Contains(template.LetterTemplateId)).ToListAsync();

        return new EventsAndLetters()
        {
            Events = events.ToDictionary(x => x.EventId),
            Accounts = await GetAccountNames(),
            Letters = letters,
            LetterTemplates = templates.ToDictionary(x => x.LetterTemplateId)
        };
    }

    public async Task<EventAttendance[]> GetAccountAttendances(string accountId)
    {
        var letters = (await _larpContext.Letters.AsQueryable()
                .Where(x => x.AccountId == accountId)
                .Select(x => new Letter() { EventId = x.EventId, Name = x.Name, State = x.State })
                .ToListAsync())
            .ToLookup(x => x.EventId);

        var attendances =
            await _larpContext.Attendances.AsQueryable()
                .Where(attendance => attendance.AccountId == accountId)
                .Join(
                    _larpContext.Events.AsQueryable(),
                    attendance => attendance.EventId,
                    @event => @event.EventId,
                    (attendance, @event) => new { Attedance = attendance, Event = @event })
                .ToListAsync();

        return attendances
            .Select(x => new EventAttendance(
                x.Attedance,
                x.Event,
                letters[x.Event.EventId].ToArray()))
            .ToArray();
    }
}