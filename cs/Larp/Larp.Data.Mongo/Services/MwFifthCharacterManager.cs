using Larp.Data.MwFifth;

namespace Larp.Data.Mongo.Services;

public class MwFifthCharacterManager
{
    private readonly LarpContext _larpContext;
    private readonly ILogger _logger;
    private readonly MwFifthGameContext _mwFifth;
    private readonly IUserSessionManager _userManager;

    public MwFifthCharacterManager(LarpContext larpContext, ILogger<MwFifthCharacterManager> logger,
        IUserSessionManager userManager)
    {
        _larpContext = larpContext;
        _logger = logger;
        _userManager = userManager;
        _mwFifth = larpContext.MwFifthGame;
    }

    private async Task<CharacterRevision?> GetLiveRevision(string? uniqueId) =>
        await _mwFifth.CharacterRevisions
            .FindOneAsync(x => x.CharacterId == uniqueId && x.State == CharacterState.Live);

    private async Task<Character> GetCharacter(string characterId, bool isAdmin)
    {
        var projection = isAdmin
            ? Builders<Character>.Projection.As<Character>()
            : Builders<Character>.Projection.Exclude(x => x.PreregistrationNotes);

        return await _mwFifth.Characters.Find(x => x.CharacterId == characterId)
                   .Project(projection).FirstOrDefaultAsync()
               ?? throw new ResourceNotFoundException();
    }

    public async Task Move(string characterId, string newAccountId)
    {
        var characterResult = await _mwFifth.Characters.UpdateOneAsync(x => x.CharacterId == characterId,
            Builders<Character>.Update.Set(x => x.AccountId, newAccountId));
        var revisionResult = await _mwFifth.CharacterRevisions.UpdateManyAsync(x => x.CharacterId == characterId,
            Builders<CharacterRevision>.Update.Set(x => x.AccountId, newAccountId));

        var oldAccount = await _mwFifth.Characters
            .Find(c => c.CharacterId == characterId)
            .Project(c => c.AccountId)
            .FirstAsync();

        await UpdateMoonstone(oldAccount);
        await UpdateMoonstone(newAccountId);

        _logger.LogInformation(
            "Moved Character {CharacterId} to Account {AccountId}: {CharacterCount} characters, {RevisionCount} revisions",
            characterId, newAccountId, characterResult.ModifiedCount, revisionResult.ModifiedCount);
    }

    public async Task MoveAll(string oldAccountId, string newAccountId)
    {
        var characterResult = await _mwFifth.Characters.UpdateManyAsync(x => x.AccountId == oldAccountId,
            Builders<Character>.Update.Set(x => x.AccountId, newAccountId));
        var revisionResult = await _mwFifth.CharacterRevisions.UpdateManyAsync(x => x.AccountId == oldAccountId,
            Builders<CharacterRevision>.Update.Set(x => x.AccountId, newAccountId));
        await _mwFifth.CharacterRevisions.UpdateManyAsync(x => x.ApprovedBy == oldAccountId,
            Builders<CharacterRevision>.Update.Set(x => x.ApprovedBy, newAccountId));

        await UpdateMoonstone(oldAccountId);
        await UpdateMoonstone(newAccountId);

        _logger.LogInformation(
            "Moved Characters from Account {FromAccountId} to Account {ToAccountId}: {CharacterCount} characters, {RevisionCount} revisions",
            oldAccountId, newAccountId, characterResult.ModifiedCount, revisionResult.ModifiedCount);
    }

    public async Task<CharacterAndRevisions> GetRevisions(string? characterId, Account account, bool isAdmin)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.RevisionId == characterId)
                .Select(x => new { x.AccountId, UniqueId = x.CharacterId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        var character = await GetCharacter(reference.UniqueId, isAdmin);

        // If not admin, fail
        if (!isAdmin && account.AccountId == reference.AccountId)
            throw new ResourceForbiddenException();

        var revisions = await _mwFifth.CharacterRevisions
            .Find(x => x.CharacterId == reference.UniqueId)
            .ToListAsync();

        var result = new List<CharacterRevision>();

        // Start with the first character revision
        var revision = revisions.First(x => x.PreviousRevisionId == null);
        result.Add(revision);

        // Add each character revision in order
        while (true)
        {
            revision = revisions.FirstOrDefault(x => x.PreviousRevisionId == revision.RevisionId);
            if (revision == null) break;
            result.Add(revision);
        }

        // Add any drafts 
        result.AddRange(revisions.Except(result));

        return new CharacterAndRevisions(character, result.ToArray(), await GetAvailableMoonstone(reference.AccountId));
    }

    private async Task<MoonstoneInfo> GetAvailableMoonstone(string accountId)
    {
        var moonstoneUsed = await _mwFifth.Characters.AsQueryable()
            .Where(character => character.AccountId == accountId)
            .SumAsync(character => character.UsedMoonstone);
        var moonstoneTotal = await this._larpContext.Attendances.AsQueryable()
            .Where(attendance => attendance.AccountId == accountId && attendance.MwFifth != null)
            .SumAsync(attendance =>
                (attendance.MwFifth!.Moonstone ?? 0)
                + (attendance.MwFifth!.PostMoonstone ?? 0));

        return new MoonstoneInfo(moonstoneTotal, moonstoneUsed);
    }

    public async Task Approve(string revisionId, string adminAccountId)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.RevisionId == revisionId)
                .Select(x => new
                {
                    x.AccountId,
                    x.State,
                    x.CharacterId,
                    x.CharacterName,
                    x.GiftMoonstone,
                    x.SkillMoonstone,
                    x.PreregistrationNotes
                })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        await _mwFifth.Characters
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId,
                Builders<Character>.Update
                    .Set(x => x.State, CharacterState.Live)
                    .Set(x => x.CharacterName, reference.CharacterName)
                    .Set(x => x.UsedMoonstone, reference.GiftMoonstone + reference.SkillMoonstone)
                    .Set(x => x.PreregistrationRevisionNotes, reference.PreregistrationNotes));

        // Mark all (hopefully only one) Live revisions as Archive
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId && x.State == CharacterState.Live,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Archived)
                    .Set(x => x.ArchivedOn, DateTime.UtcNow));

        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.RevisionId == revisionId,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Live)
                    .Set(x => x.ApprovedOn, DateTime.UtcNow)
                    .Set(x => x.ApprovedBy, adminAccountId));

        await UpdateMoonstone(reference.AccountId);
    }

    public async Task Reject(string revisionId, string? reviewerNotes)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.RevisionId == revisionId)
                .Select(x => new { x.State, UniqueId = x.CharacterId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        // Mark all (hopefully only one) Live revisions as Archive
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.CharacterId == reference.UniqueId && x.State == CharacterState.Review,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Draft)
                    .Set(x => x.RevisionReviewerNotes, reviewerNotes));
    }

    public async Task Retire(string revisionId, string adminAccountId)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.RevisionId == revisionId)
                .Select(x => new { x.State, x.CharacterId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Live)
            throw new BadRequestException("Character state must be Live");

        await _mwFifth.Characters
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId,
                Builders<Character>.Update
                    .Set(x => x.State, CharacterState.Retired));

        // Mark all (hopefully only one) Live revisions as Retired
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId && x.State == CharacterState.Live,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Retired)
                    .Set(x => x.RetiredOn, DateTime.UtcNow)
                    .Set(x => x.RetiredBy, adminAccountId));
    }

    public async Task Unretire(string revisionId)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.RevisionId == revisionId)
                .Select(x => new { x.State, x.CharacterId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Retired)
            throw new BadRequestException("Character state must be Retired");

        await _mwFifth.Characters
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId,
                Builders<Character>.Update
                    .Set(x => x.State, CharacterState.Live));

        // Mark all (hopefully only one) Retired revisions as Live
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId && x.State == CharacterState.Retired,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Live)
                    .Set(x => x.RetiredOn, null)
                    .Set(x => x.RetiredBy, null));
    }

    public async Task<CharacterAndRevision> GetRevision(string revisionId, Account account, bool isAdmin)
    {
        var revision = await _mwFifth.CharacterRevisions.FindOneAsync(x => x.RevisionId == revisionId)
                       ?? throw new ResourceNotFoundException();

        var character = await GetCharacter(revision.CharacterId, isAdmin);
        var moonstone = await GetAvailableMoonstone(character.AccountId);

        if (!isAdmin && character.AccountId != account.AccountId)
            throw new ResourceNotFoundException("Character does not belong to Account");

        return new CharacterAndRevision(character, revision, moonstone);
    }

    public async Task<CharacterAndRevision> GetDraft(string? revisionId, Account account, bool isAdmin)
    {
        var reference = await _mwFifth.CharacterRevisions.AsQueryable()
                            .Where(x => x.RevisionId == revisionId)
                            .Select(x => new { x.AccountId, UniqueId = x.CharacterId })
                            .FirstOrDefaultAsync()
                        ?? throw new ResourceNotFoundException("CharacterId was not found");

        // If not admin, fail
        if (!isAdmin && account.AccountId != reference.AccountId)
            throw new ResourceNotFoundException("Character does not belong to Account");

        var moonstone = await GetAvailableMoonstone(reference.AccountId);

        var character = await GetCharacter(reference.UniqueId, isAdmin);

        var revisions = await _mwFifth.CharacterRevisions
            .Find(x => x.CharacterId == reference.UniqueId && x.State != CharacterState.Archived)
            .ToListAsync();

        // If we already have a draft, return that
        var revision = revisions.FirstOrDefault(x => x.State == CharacterState.Draft);
        if (revision != null)
            return new CharacterAndRevision(character, revision, moonstone);

        // If character is in review, return it if admin, fail if not
        revision = revisions.FirstOrDefault(x => x.State == CharacterState.Review);
        if (isAdmin && revision != null)
            return new CharacterAndRevision(character, revision, moonstone);

        if (revision != null)
            throw new BadRequestException("Cannot modify character in review");

        revision = revisions.FirstOrDefault(x => x.State == CharacterState.Live)
                   ?? throw new ResourceNotFoundException();

        // Create new draft
        revision.PreviousRevisionId = revision.RevisionId;
        revision.RevisionId = LarpContext.GenerateNewId();
        revision.State = CharacterState.Draft;
        revision.RevisionPlayerNotes = null;
        revision.RevisionReviewerNotes = null;
        await _mwFifth.CharacterRevisions.InsertOneAsync(revision);
        return new CharacterAndRevision(character, revision, moonstone);
    }

    public async Task Save(CharacterRevision revision, Account account, bool isAdmin)
    {
        var saved = await GetRevision(revision.RevisionId, account, isAdmin);

        if (saved == null)
            throw new BadRequestException("Character does not exist");

        if (saved.Character.AccountId != revision.AccountId)
            throw new BadRequestException("Character cannot change owner");

        if (!isAdmin && saved.Revision.State != CharacterState.Draft)
            throw new BadRequestException("Character must be in draft to save changes");

        if (isAdmin && saved.Revision.State is not CharacterState.Draft and not CharacterState.Review)
            throw new BadRequestException("Character must be in draft or review to save changes");

        // User cannot change the state inappropriately
        if (revision.State is not (CharacterState.Draft or CharacterState.Review))
            throw new BadRequestException("State may only be Draft or Review");

        if (isAdmin)
            revision.RevisionPlayerNotes = saved.Revision.RevisionPlayerNotes;
        else
            revision.RevisionReviewerNotes = saved.Revision.RevisionReviewerNotes;

        // Update the change summary from the most recent live
        var live = await GetLiveRevision(revision.CharacterId);
        revision.ChangeSummary =
            live != null
                ? CharacterRevision.BuildChangeSummary(live, revision)
                : new Dictionary<string, ChangeSummary>();

        if (saved.Revision.State is not CharacterState.Review && revision.State is CharacterState.Review)
            revision.SubmittedOn = DateTime.UtcNow;

        revision.UnlockedAllSpells = saved.Revision.UnlockedAllSpells;
        revision.CreatedOn = saved.Revision.CreatedOn;
        revision.CharacterId = saved.Revision.CharacterId;

        await _mwFifth.CharacterRevisions.ReplaceOneAsync(x => x.RevisionId == revision.RevisionId, revision);
        await UpdateCharacterCount(revision.AccountId);
    }

    private async Task UpdateCharacterCount(string accountId)
    {
        var characterCount = (int)await _larpContext.MwFifthGame.Characters
            .Find(c => c.AccountId == accountId && c.State == CharacterState.Live)
            .CountDocumentsAsync();
        await _userManager.UpdateUserAccount(accountId, update => update
            .Set(x => x.CharacterCount, characterCount));
    }

    public async Task<CharacterAndRevision> GetNew(Account account)
    {
        var now = DateTime.UtcNow;
        var character = new Character
        {
            CharacterId = LarpContext.GenerateNewId(),
            AccountId = account.AccountId,
            CreatedOn = now,
            State = CharacterState.Draft
        };

        var revision = new CharacterRevision
        {
            RevisionId = LarpContext.GenerateNewId(),
            AccountId = account.AccountId,
            State = CharacterState.Draft,
            CharacterId = character.CharacterId,
            CreatedOn = now
        };

        await _mwFifth.Characters.InsertOneAsync(character);
        await _mwFifth.CharacterRevisions.InsertOneAsync(revision);
        var moonstone = await GetAvailableMoonstone(account.AccountId);

        return new CharacterAndRevision(character, revision, moonstone);
    }

    public async Task Delete(string revisionId, Account account, bool isAdmin)
    {
        var character = await GetRevision(revisionId, account, isAdmin)
                        ?? throw new ResourceNotFoundException();

        // User cannot change the state to Live
        if (character.Revision.State is not CharacterState.Draft)
            throw new BadRequestException("Character must be in draft");

        await _mwFifth.CharacterRevisions.DeleteOneAsync(x => x.RevisionId == revisionId);
    }

    public async Task<CharacterAndRevision> GetLatest(string characterId, Account account, bool isAdmin)
    {
        var reference = await _mwFifth.CharacterRevisions.AsQueryable()
            .Where(x => x.RevisionId == characterId)
            .Select(x => new { x.AccountId, UniqueId = x.CharacterId })
            .FirstOrDefaultAsync();

        // If not admin, fail
        if (!isAdmin && account.AccountId == reference.AccountId)
            throw new ResourceNotFoundException();

        var moonstone = await GetAvailableMoonstone(reference.AccountId);

        var revisions = await _mwFifth.CharacterRevisions
            .Find(x => x.CharacterId == reference.UniqueId && x.State != CharacterState.Archived)
            .ToListAsync();

        var character = await GetCharacter(reference.UniqueId, isAdmin);

        var revision =
            revisions.FirstOrDefault(x => x.State is CharacterState.Draft or CharacterState.Review)
            ?? revisions.FirstOrDefault(x => x.State == CharacterState.Live)
            ?? throw new ResourceNotFoundException();

        return new CharacterAndRevision(character, revision, moonstone);
    }

    public async Task UpdateMoonstone(string accountId)
    {
        var moonstoneUsed = await _mwFifth.Characters.AsQueryable()
            .Where(character => character.AccountId == accountId)
            .SumAsync(character => character.UsedMoonstone);
        var moonstoneTotal = await this._larpContext.Attendances.AsQueryable()
            .Where(attendance => attendance.AccountId == accountId && attendance.MwFifth != null)
            .SumAsync(attendance =>
                (attendance.MwFifth!.Moonstone ?? 0) +
                (attendance.MwFifth!.PostMoonstone ?? 0));
        var characterCount = (int)await _larpContext.MwFifthGame.Characters
            .Find(c => c.AccountId == accountId &&
                       (c.State == CharacterState.Live || c.State == CharacterState.Retired))
            .CountDocumentsAsync();
        await _userManager.UpdateUserAccount(accountId, update => update
            .Set(x => x.MwFifthMoonstone, moonstoneTotal)
            .Set(x => x.MwFifthUsedMoonstone, moonstoneUsed)
            .Set(x => x.CharacterCount, characterCount));
    }

    public static async Task UpdateMoonstone(LarpContext larpContext)
    {
        var usedMoonstone = (await larpContext.Accounts.AsQueryable()
                .Join(larpContext.MwFifthGame.Characters,
                    x => x.AccountId,
                    x => x.AccountId,
                    (account, character) => new { account, character })
                .GroupBy(x => x.account.AccountId)
                .Select(x => new
                {
                    AccountId = x.Key,
                    UsedMoonstone = x.Sum(y => y.character.UsedMoonstone)
                })
                .ToListAsync())
            .ToDictionary(x => x.AccountId);

        var totalMoonstone = (await larpContext.Attendances.AsQueryable()
                .Where(x => x.MwFifth != null && (x.MwFifth.Moonstone != null || x.MwFifth.PostMoonstone != null))
                .Join(
                    larpContext.Accounts.AsQueryable(),
                    x => x.AccountId,
                    x => x.AccountId,
                    (attendance, account) => new { attendance, account })
                .GroupBy(x => x.account.AccountId)
                .Select(x => new
                {
                    AccountId = x.Key,
                    NewMoonstone =
                        x.Sum(y => y.attendance.MwFifth!.Moonstone) +
                        x.Sum(y => y.attendance.MwFifth!.PostMoonstone)
                })
                .ToListAsync())
            .ToDictionary(x => x.AccountId);

        var characters = (await larpContext.MwFifthGame.Characters.AsQueryable()
                .Where(x => x.State == CharacterState.Live || x.State == CharacterState.Retired)
                .Join(
                    larpContext.Accounts.AsQueryable(),
                    x => x.AccountId,
                    x => x.AccountId,
                    (attendance, account) => new { attendance, account })
                .GroupBy(x => x.account.AccountId)
                .Select(x => new
                {
                    AccountId = x.Key,
                    Count = x.Count()
                })
                .ToListAsync())
            .ToDictionary(x => x.AccountId);

        var ids = usedMoonstone.Select(x => x.Key)
            .Concat(totalMoonstone.Select(x => x.Key))
            .Concat(characters.Select(x => x.Key))
            .ToHashSet();

        var updates = ids
            .Select(accountId => new UpdateOneModel<Account>(
                Builders<Account>.Filter.Eq(x => x.AccountId, accountId),
                Builders<Account>.Update
                    .Set(x => x.MwFifthMoonstone, totalMoonstone.GetValueOrDefault(accountId)?.NewMoonstone)
                    .Set(x => x.MwFifthUsedMoonstone, usedMoonstone.GetValueOrDefault(accountId)?.UsedMoonstone)
                    .Set(x => x.CharacterCount, characters.GetValueOrDefault(accountId)?.Count)))
            .ToList();

        if (updates.Count > 0)
            await larpContext.Accounts.BulkWriteAsync(updates);
    }

    public async Task<CharacterAccountSummary[]> GetState(CharacterState state)
    {
        var list = await _larpContext.MwFifthGame.CharacterRevisions.AsQueryable()
            .Where(character => character.State == state)
            .Join(_larpContext.Accounts.AsQueryable(),
                character => character.AccountId,
                account => account.AccountId,
                (character, account) => new { Character = character, Account = account })
            .ToListAsync();

        return list
            .Select(x => new CharacterAccountSummary(x.Character, x.Account))
            .ToArray();
    }

    public async Task SetNotes(string characterId, string? notes)
    {
        var result = await _larpContext.MwFifthGame.Characters.UpdateOneAsync(
            x => x.CharacterId == characterId,
            Builders<Character>.Update.Set(x => x.PreregistrationNotes, notes));
        if (result.ModifiedCount == 0)
            throw new ResourceNotFoundException($"Character {characterId} does not exist");
    }
}