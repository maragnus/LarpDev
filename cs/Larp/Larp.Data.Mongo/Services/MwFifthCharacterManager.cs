using Larp.Common.Exceptions;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Larp.Data.Mongo.Services;

public class MwFifthCharacterManager
{
    private readonly LarpContext _larpContext;
    private readonly ILogger _logger;
    private readonly MwFifthGameContext _mwFifth;

    public MwFifthCharacterManager(LarpContext larpContext, ILogger<MwFifthCharacterManager> logger)
    {
        _larpContext = larpContext;
        _logger = logger;
        _mwFifth = larpContext.MwFifthGame;
    }

    private async Task<CharacterRevision?> GetLiveRevision(string? uniqueId) =>
        await _mwFifth.CharacterRevisions
            .FindOneAsync(x => x.CharacterId == uniqueId && x.State == CharacterState.Live);

    private async Task<Character> GetCharacter(string characterId) =>
        await _mwFifth.Characters.FindOneAsync(x => x.CharacterId == characterId)
        ?? throw new ResourceNotFoundException();

    public async Task Move(string characterId, string newAccountId)
    {
        var characterResult = await _mwFifth.Characters.UpdateOneAsync(x => x.CharacterId == characterId,
            Builders<Character>.Update.Set(x => x.AccountId, newAccountId));
        var revisionResult = await _mwFifth.CharacterRevisions.UpdateManyAsync(x => x.CharacterId == characterId,
            Builders<CharacterRevision>.Update.Set(x => x.AccountId, newAccountId));

        _logger.LogInformation(
            "Moved Character {CharacterId} to Account {AccountId}: {CharacterCount} characters, {RevisionCount} revisions",
            characterId, newAccountId, characterResult.ModifiedCount, revisionResult.ModifiedCount);
    }

    public async Task MoveAll(string oldAccountId, string newAccountId)
    {
        var characterResult = await _mwFifth.Characters.UpdateOneAsync(x => x.AccountId == oldAccountId,
            Builders<Character>.Update.Set(x => x.AccountId, newAccountId));
        var revisionResult = await _mwFifth.CharacterRevisions.UpdateManyAsync(x => x.AccountId == oldAccountId,
            Builders<CharacterRevision>.Update.Set(x => x.AccountId, newAccountId));

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

        var character = await GetCharacter(reference.UniqueId);

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
            .SumAsync(attendance => attendance.MwFifth!.Moonstone ?? 0);

        return new MoonstoneInfo(moonstoneTotal, moonstoneUsed);
    }

    public async Task Approve(string characterId, string adminAccountId)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.RevisionId == characterId)
                .Select(x => new
                {
                    x.AccountId,
                    x.State,
                    x.CharacterId,
                    x.CharacterName,
                    x.GiftMoonstone,
                    x.SkillMoonstone
                })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        await _mwFifth.Characters
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId,
                Builders<Character>.Update
                    .Set(x => x.CharacterName, reference.CharacterName)
                    .Set(x => x.UsedMoonstone, reference.GiftMoonstone + reference.SkillMoonstone));

        // Mark all (hopefully only one) Live characters are Archive
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.CharacterId == reference.CharacterId && x.State == CharacterState.Live,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Archived)
                    .Set(x => x.ArchivedOn, DateTime.UtcNow));

        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.RevisionId == characterId,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Live)
                    .Set(x => x.ApprovedOn, DateTime.UtcNow)
                    .Set(x => x.ApprovedBy, adminAccountId));

        await UpdateMoonstone(reference.AccountId);
    }

    public async Task Reject(string characterId)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.RevisionId == characterId)
                .Select(x => new { x.State, UniqueId = x.CharacterId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        // Mark all (hopefully only one) Live characters are Archive
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.CharacterId == reference.UniqueId && x.State == CharacterState.Review,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Draft));
    }

    public async Task<CharacterAndRevision> GetRevision(string revisionId, Account account, bool isAdmin)
    {
        var revision = await _mwFifth.CharacterRevisions.FindOneAsync(x => x.RevisionId == revisionId)
                       ?? throw new ResourceNotFoundException();

        var character = await GetCharacter(revision.CharacterId);
        var moonstone = await GetAvailableMoonstone(character.AccountId);

        if (!isAdmin && character.AccountId != account.AccountId)
            throw new ResourceNotFoundException("Character does not belong to Account");

        return new CharacterAndRevision(character, revision, moonstone);
    }

    public async Task<CharacterAndRevision> GetDraft(string? characterId, Account account, bool isAdmin)
    {
        var reference = await _mwFifth.CharacterRevisions.AsQueryable()
                            .Where(x => x.RevisionId == characterId)
                            .Select(x => new { x.AccountId, UniqueId = x.CharacterId })
                            .FirstOrDefaultAsync()
                        ?? throw new ResourceNotFoundException("CharacterId was not found");

        // If not admin, fail
        if (!isAdmin && account.AccountId != reference.AccountId)
            throw new ResourceNotFoundException("Character does not belong to Account");

        var moonstone = await GetAvailableMoonstone(reference.AccountId);

        var character = await GetCharacter(reference.UniqueId);

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
        revision.RevisionId = ObjectId.GenerateNewId().ToString();
        revision.State = CharacterState.Draft;
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
    }

    public async Task<CharacterAndRevision> GetNew(Account account)
    {
        var now = DateTime.UtcNow;
        var character = new Character()
        {
            CharacterId = ObjectId.GenerateNewId().ToString(),
            AccountId = account.AccountId,
            CreatedOn = now,
        };

        var revision = new CharacterRevision
        {
            RevisionId = ObjectId.GenerateNewId().ToString(),
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

    public async Task Delete(string characterId, Account account, bool isAdmin)
    {
        var character = await GetRevision(characterId, account, isAdmin)
                        ?? throw new ResourceNotFoundException();

        // User cannot change the state to Live
        if (character.Revision.State is not CharacterState.Draft)
            throw new BadRequestException("Character must be in draft");

        await _mwFifth.CharacterRevisions.DeleteOneAsync(x => x.RevisionId == characterId);
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

        var character = await GetCharacter(reference.UniqueId);

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
            .SumAsync(attendance => attendance.MwFifth!.Moonstone ?? 0);
        await _larpContext.Accounts.UpdateOneAsync(x => x.AccountId == accountId,
            Builders<Account>.Update
                .Set(x => x.MwFifthMoonstone, moonstoneTotal)
                .Set(x => x.MwFifthUsedMoonstone, moonstoneUsed));
    }

    public async Task UpdateMoonstone()
    {
        var usedMoonstone = (await _larpContext.Accounts.AsQueryable()
                .Join(_larpContext.MwFifthGame.Characters,
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

        var totalMoonstone = (await _larpContext.Attendances.AsQueryable()
                .Where(x => x.MwFifth != null && x.MwFifth.Moonstone != null)
                .Join(
                    _larpContext.Accounts.AsQueryable(),
                    x => x.AccountId,
                    x => x.AccountId,
                    (attendance, account) => new { attendance, account })
                .GroupBy(x => x.account.AccountId)
                .Select(x => new
                {
                    AccountId = x.Key,
                    NewMoonstone = x.Sum(y => y.attendance.MwFifth!.Moonstone)
                })
                .ToListAsync())
            .ToDictionary(x => x.AccountId);

        var ids = usedMoonstone.Select(x => x.Key)
            .Concat(totalMoonstone.Select(x => x.Key))
            .ToHashSet();

        var updates = ids
            .Select(accountId => new UpdateOneModel<Account>(
                Builders<Account>.Filter.Eq(x => x.AccountId, accountId),
                Builders<Account>.Update
                    .Set(x => x.MwFifthMoonstone, totalMoonstone.GetValueOrDefault(accountId)?.NewMoonstone)
                    .Set(x => x.MwFifthUsedMoonstone, usedMoonstone.GetValueOrDefault(accountId)?.UsedMoonstone)))
            .ToList();

        await _larpContext.Accounts.BulkWriteAsync(updates);
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
}