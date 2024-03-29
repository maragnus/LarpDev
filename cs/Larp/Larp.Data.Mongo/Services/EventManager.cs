using Larp.Data.MwFifth;

namespace Larp.Data.Mongo.Services;

public class EventManager
{
    private readonly MwFifthCharacterManager _characterManager;
    private readonly LarpContext _larpContext;

    public EventManager(LarpContext larpContext, MwFifthCharacterManager characterManager)
    {
        _larpContext = larpContext;
        _characterManager = characterManager;
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
            .Project(account => new AccountName
            {
                AccountId = account.AccountId,
                State = account.State,
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
                .Set(x => x.Summary, @event.Summary)
                .Set(x => x.Clarifies, @event.Clarifies)
                .Set(x => x.IsHidden, @event.IsHidden)
                .Set(x => x.LetterTemplates, @event.LetterTemplates)
                .Set(x => x.Components, @event.Components)
                .Set(x => x.PreregistrationNotes, @event.PreregistrationNotes)
                .Set(x => x.AdminNotes, @event.AdminNotes)
                .Set(x => x.EventCost, @event.EventCost)
                .Set(x => x.ChronicleCost, @event.ChronicleCost)
        );
    }

    public async Task DeleteEvent(string eventId)
    {
        var attendees = await _larpContext.Attendances.CountDocumentsAsync(x => x.EventId == eventId);
        if (attendees > 0)
            throw new BadRequestException($"Cannot delete event, it has {attendees} attendees");
        await _larpContext.Events.DeleteOneAsync(x => x.EventId == eventId);
    }

    public async Task SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone1,
        int? moonstone2, int? paid, int? cost, string[] characterIds)
    {
        if (!attended)
        {
            await _larpContext.Attendances.DeleteOneAsync(x => x.EventId == eventId && x.AccountId == accountId);
            await UpdateAttendance(eventId);
            return;
        }

        var update = Builders<Attendance>.Update
            .SetOnInsert(x => x.EventId, eventId)
            .SetOnInsert(x => x.AccountId, accountId)
            .SetOnInsert(x => x.CreatedOn, DateTimeOffset.Now)
            .SetOnInsert(x => x.UpdatedOn, DateTimeOffset.Now)
            .Set(x => x.Paid, paid)
            .Set(x => x.Cost, cost)
            .Set(x => x.MwFifth,
                new MwFifthAttendance
                {
                    Moonstone = moonstone1,
                    PostMoonstone = moonstone2,
                    CharacterIds = characterIds
                });
        var upsert = new UpdateOptions() { IsUpsert = true };

        await _larpContext.Attendances.UpdateOneAsync(
            attendance => attendance.EventId == eventId && attendance.AccountId == accountId,
            update,
            upsert);

        await _characterManager.UpdateMoonstone(accountId);
        await UpdateAttendance(eventId);
    }

    public async Task<Attendance[]> GetEventAttendances(string eventId) =>
        (await _larpContext.Attendances
            .Find(attendance => attendance.EventId == eventId)
            .ToListAsync())
        .ToArray();
    
    public async Task UpdateAttendance(string eventId)
    {
        var attendees = (int?)await _larpContext.Attendances.CountDocumentsAsync(x => x.EventId == eventId);
        await _larpContext.Events
            .UpdateOneAsync(x => x.EventId == eventId, Builders<Event>.Update
                .Set(x => x.Attendees, attendees));
    }

    public async Task<Event> DraftEvent()
    {
        var id = LarpContext.GenerateNewId();
        var e = new Event()
        {
            EventId = id,
            Title = "New Event",
            Date = DateOnly.FromDateTime(DateTime.Today),
            EventType = "Game",
            GameId = await _larpContext.Games.Find(x => x.Name == GameState.GameName).Project(x => x.Id)
                         .FirstOrDefaultAsync()
                     ?? throw new BadRequestException("GameId could not be found"),
            IsHidden = true
        };
        await _larpContext.Events.InsertOneAsync(e);
        return e;
    }

    public async Task<EventAttendanceList> GetEvents(EventList list, string? accountId)
    {
        var now = DateOnly.FromDateTime(DateTime.Today);

        var id = accountId ?? "";

        var eventFilter = list switch
        {
            EventList.All => Builders<Event>.Filter.Empty,
            EventList.Past => Builders<Event>.Filter.Where(x => !x.IsHidden && x.Date <= now.AddDays(4)),
            EventList.Upcoming => Builders<Event>.Filter.Where(x => !x.IsHidden && x.Date >= now.AddDays(-4)),
            EventList.Dashboard => Builders<Event>.Filter.Where(x => !x.IsHidden && x.Date >= now.AddMonths(-6)),
            _ => throw new ArgumentOutOfRangeException(nameof(list), list, null)
        };

        var events = await _larpContext.Events
            .Find(eventFilter)
            .Project<Event>(Builders<Event>.Projection
                .Exclude(@event => @event.AdminNotes)
                .Exclude(@event => @event.PreregistrationNotes))
            .ToListAsync();

        var eventIds = events.Select(@event => @event.EventId).ToArray();

        var attendances =
            (await _larpContext.Attendances
                .Find(Builders<Attendance>.Filter.And(
                    Builders<Attendance>.Filter.Eq(attendance => attendance.AccountId, accountId),
                    Builders<Attendance>.Filter.In(attendance => attendance.EventId, eventIds)
                ))
                .ToListAsync())
            .ToDictionary(attendance => attendance.EventId);

        var letters =
            (await _larpContext.Letters
                .Find(Builders<Letter>.Filter.And(
                    Builders<Letter>.Filter.Eq(letter => letter.AccountId, accountId),
                    Builders<Letter>.Filter.In(letter => letter.EventId, eventIds)
                ))
                .ToListAsync())
            .ToDictionary(letter => (letter.EventId, letter.Name));

        var templateIds = events
            .SelectMany(ev => ev.LetterTemplates)
            .Select(ev => ev.LetterTemplateId)
            .Distinct()
            .ToList();

        var templates =
            (await _larpContext.LetterTemplates
                .Find(Builders<LetterTemplate>.Filter.In(template => template.LetterTemplateId, templateIds))
                .ToListAsync());

        return new EventAttendanceList()
        {
            AccountId = accountId,
            Events = (from @event in events
                let eventId = @event.EventId
                select new EventAttendanceItem()
                {
                    Event = @event,
                    Attendance = attendances.GetValueOrDefault(eventId),
                    PreEvent = letters.GetValueOrDefault((eventId, LetterNames.PreEvent)),
                    PostEvent = letters.GetValueOrDefault((eventId, LetterNames.PostEvent)),
                    BetweenEvent = letters.GetValueOrDefault((eventId, LetterNames.BetweenEvent)),
                }).ToArray(),
            LetterTemplates = templates.ToDictionary(l => l.LetterTemplateId)
        };
    }

    public async Task<EventAttendanceList> GetAccountAttendances(string accountId)
    {
        var letters = (await _larpContext.Letters.AsQueryable()
                .Where(x => x.AccountId == accountId)
                .Select(x => new Letter() { EventId = x.EventId, Name = x.Name, State = x.State })
                .ToListAsync())
            .ToDictionary(letter => (letter.EventId, letter.Name));

        var attendances =
            await _larpContext.Attendances.AsQueryable()
                .Where(attendance => attendance.AccountId == accountId)
                .Join(
                    _larpContext.Events.AsQueryable(),
                    attendance => attendance.EventId,
                    @event => @event.EventId,
                    (attendance, @event) => new { Attedance = attendance, Event = @event })
                .ToListAsync();

        // Clear admin-only info
        attendances.ForEach(x =>
        {
            x.Event.PreregistrationNotes = default;
            x.Event.AdminNotes = default;
        });

        var templateIds = letters
            .Select(x => x.Value.TemplateId)
            .Distinct().ToArray();
        var templates = await _larpContext.LetterTemplates
            .Find(template => templateIds.Contains(template.LetterTemplateId)).ToListAsync();

        return new EventAttendanceList
        {
            AccountId = accountId,
            Events = attendances.Select(a => new EventAttendanceItem
            {
                Event = a.Event,
                Attendance = a.Attedance,
                PreEvent = letters.GetValueOrDefault((a.Event.EventId, LetterNames.PreEvent)),
                PostEvent = letters.GetValueOrDefault((a.Event.EventId, LetterNames.PostEvent)),
                BetweenEvent = letters.GetValueOrDefault((a.Event.EventId, LetterNames.BetweenEvent)),
            }).ToArray(),
            LetterTemplates = templates.ToDictionary(l => l.LetterTemplateId)
        };
    }
}