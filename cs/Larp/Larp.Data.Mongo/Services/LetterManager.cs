namespace Larp.Data.Mongo.Services;

public class LetterManager
{
    private readonly LarpContext _larpContext;

    public LetterManager(LarpContext larpContext)
    {
        _larpContext = larpContext;
    }

    private async Task<Letter> Draft(string letterTemplateId, string eventId, string accountId, string letterName)
    {
        var letter = new Letter
        {
            LetterId = LarpContext.GenerateNewId(),
            AccountId = accountId,
            EventId = eventId,
            Name = letterName,
            TemplateId = letterTemplateId,
            State = LetterState.NotStarted,
            StartedOn = DateTimeOffset.Now,
            ChangeLog = new[] { ChangeLog.Log("Draft", LetterState.NotStarted, accountId) }
        };
        await _larpContext.Letters.InsertOneAsync(letter);
        return letter;
    }

    public async Task AdminSave(Letter letter, string adminAccountId)
    {
        var oldLetter = await Get(letter.LetterId, adminAccountId, isAdmin: true);

        if (letter.State == LetterState.NotStarted)
            letter.State = LetterState.Draft;

        var update = Builders<Letter>.Update
            .Set(x => x.Fields, letter.Fields)
            .Set(x => x.State, letter.State);

        if (oldLetter.StartedOn == null)
            update = update.Set(x => x.StartedOn, DateTimeOffset.Now);

        if (letter.State == LetterState.Submitted)
            update = update.Set(x => x.SubmittedOn, DateTimeOffset.Now);

        update = update.Push(x => x.ChangeLog, ChangeLog.Log("Admin Save", letter.State, adminAccountId));

        await _larpContext.Letters.UpdateOneAsync(l => l.LetterId == letter.LetterId, update);
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

        update = update.Push(x => x.ChangeLog, ChangeLog.Log("Save", letter.State, accountId));

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
            .Set(x => x.State, LetterState.Approved)
            .Push(x => x.ChangeLog, ChangeLog.Log("Approve", LetterState.Approved, adminAccountId));

        await _larpContext.Letters.UpdateOneAsync(l => l.LetterId == letterId, update);
    }

    public async Task Reject(string letterId, string adminAccountId)
    {
        var update = Builders<Letter>.Update
            .Set(x => x.ApprovedOn, null)
            .Set(x => x.ApprovedBy, null)
            .Set(x => x.SubmittedOn, null)
            .Set(x => x.State, LetterState.Draft)
            .Push(x => x.ChangeLog, ChangeLog.Log("Reject", LetterState.Draft, adminAccountId));

        await _larpContext.Letters.UpdateOneAsync(l => l.LetterId == letterId, update);
    }

    public async Task Unapprove(string letterId, string adminAccountId)
    {
        var update = Builders<Letter>.Update
            .Set(x => x.ApprovedOn, null)
            .Set(x => x.ApprovedBy, null)
            .Set(x => x.State, LetterState.Submitted)
            .Push(x => x.ChangeLog, ChangeLog.Log("Unapprove", LetterState.Submitted, adminAccountId));

        await _larpContext.Letters.UpdateOneAsync(l => l.LetterId == letterId, update);
    }

    public async Task<LetterTemplate> GetTemplate(string templateId) =>
        await _larpContext.LetterTemplates.FindOneAsync(x => x.LetterTemplateId == templateId)
        ?? throw new ResourceNotFoundException();

    public async Task<LetterTemplate> DraftTemplate()
    {
        var template = new LetterTemplate()
        {
            LetterTemplateId = LarpContext.GenerateNewId(),
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

    public async Task<EventsAndLetters> GetEventLetter(string letterId)
    {
        var letter =
            await _larpContext.Letters.Find(l => l.LetterId == letterId).FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        var accountNameTask =
            _larpContext.Accounts
                .Find(x => x.AccountId == letter.AccountId || x.AccountId == letter.ApprovedBy)
                .Project(x => new AccountName
                {
                    AccountId = x.AccountId,
                    State = x.State,
                    Name = x.Name
                })
                .ToListAsync();

        var @event =
            await _larpContext.Events
                .Find(x => x.EventId == letter.EventId)
                .FirstOrDefaultAsync();

        var templateId =
            @event.LetterTemplates
                .FirstOrDefault(x => x.Name == letter.Name)
            ?? throw new BadRequestException($"Event {letter.EventId} does not have letter named {letter.Name}");

        var template = await GetTemplate(templateId.LetterTemplateId);

        var accountNames = await accountNameTask;
        return new EventsAndLetters
        {
            Accounts = accountNames.ToDictionary(an => an.AccountId),
            Events = { { @event.EventId, @event } },
            Letters = { { letter.LetterId, letter } },
            LetterTemplates = { { template.LetterTemplateId, template } },
        };
    }


    public async Task<EventsAndLetters> GetEventLetter(string accountId, string eventId, string letterName)
    {
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

        var accountNames =
            await _larpContext.Accounts
                .Find(x => x.AccountId == letter.AccountId || x.AccountId == letter.ApprovedBy)
                .Project(x => new AccountName
                {
                    AccountId = x.AccountId,
                    State = x.State,
                    Name = x.Name
                })
                .ToListAsync();

        return new EventsAndLetters()
        {
            Accounts = accountNames.ToDictionary(an => an.AccountId),
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