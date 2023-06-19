namespace Larp.Data.Mongo.Services;

public class CitationManager
{
    private readonly LarpContext _larpContext;
    private readonly UserSessionManager _userSessionManager;

    public CitationManager(LarpContext larpContext, UserSessionManager userSessionManager)
    {
        _larpContext = larpContext;
        _userSessionManager = userSessionManager;
    }

    public async Task<Citation[]> GetCitations(string accountId)
    {
        return await _larpContext.Citations.Find(c => c.AccountId == accountId)
            .ToArrayAsync();
    }

    public async Task Create(Citation citation, string authorAccountId)
    {
        await _larpContext.Citations.InsertOneAsync(new Citation()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AccountId = citation.AccountId,
            AuthorAccountId = authorAccountId,
            Title = citation.Title,
            Description = citation.Description,
            Type = citation.Type,
            State = citation.State
        });

        var count = (int)await _larpContext.Citations
            .Find(c => c.AccountId == citation.AccountId && c.State == CitationState.Open)
            .CountDocumentsAsync();
        await _userSessionManager.UpdateUserAccount(citation.AccountId,
            update => update.Set(account => account.CitationCount, count));
    }

    public async Task Edit(Citation citation, string authorAccountId)
    {
        if (citation.AuthorAccountId != authorAccountId)
            throw new BadRequestException("Cannot update another person's citation");
        await _larpContext.Citations.UpdateOneAsync(
            c => c.Id == citation.Id,
            Builders<Citation>.Update
                .Set(c => c.Title, citation.Title)
                .Set(c => c.Description, citation.Description)
                .Set(c => c.Type, citation.Type)
                .Set(c => c.State, citation.State)
        );
    }
}