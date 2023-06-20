namespace Larp.Data.Mongo.Services;

public class CitationManager
{
    private readonly LarpContext _larpContext;
    private readonly IUserSessionManager _userSessionManager;

    public CitationManager(LarpContext larpContext, IUserSessionManager userSessionManager)
    {
        _larpContext = larpContext;
        _userSessionManager = userSessionManager;
    }

    public async Task<Citation[]> GetCitations(string accountId)
    {
        return await _larpContext.Citations.Find(c => c.AccountId == accountId)
            .ToArrayAsync();
    }

    public async Task Save(Citation citation, string authorAccountId)
    {
        if (!string.IsNullOrEmpty(citation.Id))
            await Update(citation, authorAccountId);
        else
            await Create(citation, authorAccountId);
    }

    private async Task Create(Citation citation, string authorAccountId)
    {
        await _larpContext.Citations.InsertOneAsync(new Citation()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AccountId = citation.AccountId,
            AuthorAccountId = authorAccountId,
            Title = citation.Title,
            Description = citation.Description,
            Type = citation.Type,
            State = CitationState.Open,
            CreatedOn = DateTime.Today,
            ModifiedOn = DateTime.Today
        });

        var count = (int)await _larpContext.Citations
            .Find(c => c.AccountId == citation.AccountId && c.State == CitationState.Open)
            .CountDocumentsAsync();
        await _userSessionManager.UpdateUserAccount(citation.AccountId,
            update => update.Set(account => account.CitationCount, count));
    }

    private async Task Update(Citation citation, string authorAccountId)
    {
        var oldAuthorAccountId = await _larpContext.Citations.Find(c => c.Id == citation.Id)
                                     .Project(c => c.AuthorAccountId)
                                     .FirstOrDefaultAsync()
                                 ?? throw new ResourceNotFoundException("Citation was not found");
        if (citation.AuthorAccountId != oldAuthorAccountId || citation.AuthorAccountId != authorAccountId)
            throw new BadRequestException("Cannot update another person's citation");

        if (citation.State is not (CitationState.Open or CitationState.Resolved))
            citation.State = CitationState.Open;
        if (citation is { State: CitationState.Resolved, ResolvedOn: null })
            citation.ResolvedOn = DateTime.Today;

        await _larpContext.Citations.UpdateOneAsync(
            c => c.Id == citation.Id,
            Builders<Citation>.Update
                .Set(c => c.Title, citation.Title)
                .Set(c => c.Description, citation.Description)
                .Set(c => c.Type, citation.Type)
                .Set(c => c.State, citation.State)
                .Set(c => c.ModifiedOn, DateTime.Today)
                .Set(c => c.ResolvedOn, citation.ResolvedOn)
        );
    }

    public async Task SetState(string citationId, CitationState state, string authorAccountId)
    {
        switch (state)
        {
            case CitationState.Resolved:
                await _larpContext.Citations.UpdateOneAsync(
                    c => c.Id == citationId
                         && c.AuthorAccountId == authorAccountId
                         && c.State != CitationState.Resolved,
                    Builders<Citation>.Update
                        .Set(c => c.State, CitationState.Resolved)
                        .Set(c => c.ResolvedOn, DateTime.Today));
                break;
            case CitationState.Open:
                await _larpContext.Citations.UpdateOneAsync(
                    c => c.Id == citationId
                         && c.AuthorAccountId == authorAccountId
                         && c.State == CitationState.Resolved,
                    Builders<Citation>.Update
                        .Set(c => c.State, CitationState.Open)
                        .Set(c => c.ResolvedOn, null));
                break;
            case CitationState.Draft:
            default:
                throw new BadRequestException("Citation cannot be updated");
        }
    }
}