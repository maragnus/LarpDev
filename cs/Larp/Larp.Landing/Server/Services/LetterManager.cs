using Larp.Data;
using Larp.Data.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class LetterManager
{
    private readonly LarpContext _larpContext;
    private readonly ILogger<LetterManager> _logger;

    public LetterManager(LarpContext larpContext, ILogger<LetterManager> logger)
    {
        _larpContext = larpContext;
        _logger = logger;
    }

    public async Task<Letter> Draft(string letterTemplateId, string eventId, string accountId)
    {
        var letter = new Letter
        {
            LetterId = ObjectId.GenerateNewId().ToString(),
            AccountId = accountId,
            EventId = eventId,
            TemplateId = letterTemplateId,
            State = LetterState.Draft,
            StartedOn = DateTimeOffset.Now
        };
        await _larpContext.Letters.InsertOneAsync(letter);
        return letter;
    }

    public async Task Save(Letter letter, string accountId)
    {
        var oldLetter = await Get(letter.LetterId, accountId, isAdmin: false);

        if (oldLetter.State != LetterState.Draft)
            throw new BadRequestException("Cannot update letter after submitted");

        if (letter.State == LetterState.Approved)
            throw new BadRequestException("Admin is required to approve");

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

    public async Task<LettersAndTemplate> GetByEvent(string eventId)
    {
        var @event = await _larpContext.Events.FindOneAsync(x => x.EventId == eventId)
                     ?? throw new ResourceNotFoundException();

        return new LettersAndTemplate()
        {
            Event = @event,
            LetterTemplate = @event.LetterTemplateId == null ? null : await GetTemplate(@event.LetterTemplateId!),
            Letters = (await _larpContext.Letters.Find(letter => letter.EventId == eventId).ToListAsync()).ToArray()
        };
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

    public async Task<LetterAndTemplate> GetEventLetter(string accountId, string eventId)
    {
        var accountName = _larpContext.Accounts.Find(x => x.AccountId == accountId).Project(x => x.Name)
            .FirstOrDefaultAsync();
        var @event = _larpContext.Events.FindOneAsync(x => x.EventId == eventId);
        var templateId = await _larpContext.Events.Find(x => x.EventId == eventId).Project(x => x.LetterTemplateId)
                             .FirstOrDefaultAsync()
                         ?? throw new BadRequestException("Event does not have a Letter Template associated with it");
        var template = GetTemplate(templateId);
        var letter =
            await _larpContext.Letters.FindOneAsync(x => x.AccountId == accountId && x.TemplateId == templateId)
            ?? await Draft(templateId, eventId, accountId);

        return new LetterAndTemplate()
        {
            AccountId = letter.AccountId,
            AccountName = await accountName ?? "No Name Set",
            Letter = letter,
            LetterTemplate = await template,
            Event = await @event ?? throw new ResourceNotFoundException()
        };
    }
}