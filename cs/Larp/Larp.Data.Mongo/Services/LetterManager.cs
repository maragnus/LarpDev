namespace Larp.Data.Mongo.Services;

public class LetterManager
{
    private readonly LarpContext _larpContext;
    private readonly ILogger<LetterManager> _logger;

    public LetterManager(LarpContext larpContext, ILogger<LetterManager> logger)
    {
        _larpContext = larpContext;
        _logger = logger;
    }

    public async Task<Letter> Draft(string letterTemplateId, string eventId, string accountId, string letterName)
    {
        var letter = new Letter
        {
            LetterId = ObjectId.GenerateNewId().ToString(),
            AccountId = accountId,
            EventId = eventId,
            Name = letterName,
            TemplateId = letterTemplateId,
            State = LetterState.NotStarted,
            StartedOn = DateTimeOffset.Now
        };
        await _larpContext.Letters.InsertOneAsync(letter);
        return letter;
    }

    public async Task Save(Letter letter, string accountId)
    {
        var oldLetter = await Get(letter.LetterId, accountId, isAdmin: false);

        if (oldLetter.State is not LetterState.Draft and not LetterState.NotStarted)
            throw new BadRequestException("Cannot update letter after submitted");

        if (letter.State == LetterState.Approved)
            throw new BadRequestException("Admin is required to approve");

        if (letter.State == LetterState.NotStarted)
            letter.State = LetterState.Draft;

        var update = Builders<Letter>.Update
            .Set(x => x.Fields, letter.Fields)
            .Set(x => x.State, letter.State);

        if (oldLetter.StartedOn == null)
            update = update.Set(x => x.StartedOn, DateTimeOffset.Now);

        if (letter.State == LetterState.Submitted)
            update = update.Set(x => x.SubmittedOn, DateTimeOffset.Now);

        await _larpContext.Letters.UpdateOneAsync(l => l.LetterId == letter.LetterId, update);
    }

    public async Task<Letter> Get(string letterId, string accountId, bool isAdmin) =>
        await _larpContext.Letters.FindOneAsync(x => x.LetterId == letterId && (isAdmin || x.AccountId == accountId))
        ?? throw new ResourceNotFoundException();

    public async Task<Letter[]> GetAll(string accountId) =>
        (await _larpContext.Letters.Find(x => x.LetterId == accountId)
            .ToListAsync()).ToArray();

    public async Task Approve(string letterId, string adminAccountId)
    {
        var update = Builders<Letter>.Update
            .Set(x => x.ApprovedOn, DateTimeOffset.Now)
            .Set(x => x.ApprovedBy, adminAccountId)
            .Set(x => x.State, LetterState.Approved);

        await _larpContext.Letters.UpdateOneAsync(l => l.LetterId == letterId, update);
    }

    public async Task Reject(string letterId, string adminAccountId)
    {
        var update = Builders<Letter>.Update
            .Set(x => x.ApprovedOn, null)
            .Set(x => x.ApprovedBy, null)
            .Set(x => x.State, LetterState.Draft);

        await _larpContext.Letters.UpdateOneAsync(l => l.LetterId == letterId, update);
    }

    public async Task<LetterTemplate> GetTemplate(string templateId) =>
        await _larpContext.LetterTemplates.FindOneAsync(x => x.LetterTemplateId == templateId)
        ?? throw new ResourceNotFoundException();

    public async Task<LetterTemplate> DraftTemplate()
    {
        var template = new LetterTemplate()
        {
            LetterTemplateId = ObjectId.GenerateNewId().ToString(),
            Name = "new-template",
            Title = "New Template",
            Fields = Array.Empty<LetterField>()
        };
        await _larpContext.LetterTemplates.InsertOneAsync(template);
        return template;
    }

    public async Task SaveTemplate(LetterTemplate template)
    {
        var update = Builders<LetterTemplate>.Update
            .Set(x => x.Name, template.Name)
            .Set(x => x.Title, template.Title)
            .Set(x => x.Description, template.Description)
            .Set(x => x.Fields, template.Fields);

        await _larpContext.LetterTemplates.UpdateOneAsync(l => l.LetterTemplateId == template.LetterTemplateId, update);
    }

    public async Task<LetterTemplate[]> GetTemplates() =>
        (await _larpContext.LetterTemplates.Find(_ => true).ToListAsync()).ToArray();

    public async Task<Letter[]> GetByState(LetterState state) =>
        (await _larpContext.Letters.Find(letter => letter.State == state).ToListAsync()).ToArray();

    public async Task<Dictionary<string, AccountName>> GetAccountNames()
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

    public async Task<Letter[]> GetByTemplate(string templateId) =>
        (await _larpContext.Letters.Find(letter => letter.TemplateId == templateId).ToListAsync()).ToArray();

    public async Task<LetterTemplate[]> GetTemplateNames() =>
        (await _larpContext.LetterTemplates.Find(_ => true)
            .Project(x => new LetterTemplate()
            {
                LetterTemplateId = x.LetterTemplateId,
                Name = x.Name,
                Title = x.Title
            }).ToListAsync()).ToArray();

    public async Task<EventsAndLetters> GetByEvent(string eventId)
    {
        var @event = await _larpContext.Events.FindOneAsync(x => x.EventId == eventId)
                     ?? throw new ResourceNotFoundException();
        var letters = await _larpContext.Letters.Find(letter => letter.EventId == eventId).ToListAsync();
        var templateIds = letters.Select(x => x.TemplateId).Distinct().ToList();
        var templates = await _larpContext.LetterTemplates
            .Find(template => templateIds.Contains(template.LetterTemplateId)).ToListAsync();

        return new EventsAndLetters()
        {
            Accounts = await GetAccountNames(),
            Events = { { @eventId, @event } },
            LetterTemplates = templates.ToDictionary(x => x.LetterTemplateId),
            Letters = letters.ToDictionary(x => x.LetterId)
        };
    }

    public async Task<EventsAndLetters> GetEventLetter(string accountId, string eventId, string letterName)
    {
        var accountNameTask =
            _larpContext.Accounts
                .Find(x => x.AccountId == accountId)
                .Project(x => new AccountName
                {
                    AccountId = x.AccountId, 
                    State = x.State,
                    Name = x.Name
                })
                .FirstOrDefaultAsync();
        
        var @event =
            await _larpContext.Events
                .Find(x => x.EventId == eventId)
                .FirstOrDefaultAsync();
        
        var templateId =
            @event.LetterTemplates
                .FirstOrDefault(x => x.Name == letterName)
            ?? throw new BadRequestException($"Event {eventId} does not have letter named {letterName}");

        var letter =
            await _larpContext.Letters.FindOneAsync(x =>
                x.AccountId == accountId && x.EventId == eventId && x.Name == letterName &&
                x.Name == letterName);

        if (letter == null && !templateId.IsOpen)
            throw new BadRequestException($"Event {eventId} letter named {letterName} is currently closed");

        letter ??= await Draft(templateId.LetterTemplateId, eventId, accountId, templateId.Name);

        var template = await GetTemplate(templateId.LetterTemplateId);

        var accountName = await accountNameTask;
        return new EventsAndLetters()
        {
            Accounts = { { accountName.AccountId, accountName } },
            Events = { { @event.EventId, @event } },
            Letters = { { letter.LetterId, letter } },
            LetterTemplates = { { template.LetterTemplateId, template } },
        };
    }

    public async Task<Letter> Draft(string accountId, string eventId, string letterName)
    {
        var templates =
            await _larpContext.Events.Find(x => x.EventId == eventId)
                .Project(x => x.LetterTemplates)
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        var templateId =
            templates.FirstOrDefault(x => x.Name == letterName)?.LetterTemplateId
            ?? throw new BadRequestException($"Event {eventId} does not have letter {letterName}");
        return await Draft(templateId, eventId, accountId, letterName);
    }
}