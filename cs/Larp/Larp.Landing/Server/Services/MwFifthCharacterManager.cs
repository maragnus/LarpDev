using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.MwFifth;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OfficeOpenXml.Packaging.Ionic.Zip;

namespace Larp.Landing.Server.Services;

public class MwFifthCharacterManager
{
    private readonly LarpContext _larpContext;
    private readonly MwFifthGameContext _mwFifth;

    public MwFifthCharacterManager(LarpContext larpContext)
    {
        _larpContext = larpContext;
        _mwFifth = larpContext.MwFifthGame;
    }

    private async Task<CharacterRevision?> GetLiveByUniqueId(string? uniqueId) =>
        await _mwFifth.CharacterRevisions
            .FindOneAsync(x => x.UniqueId == uniqueId && x.State == CharacterState.Live);

    private async Task<Character> GetCharacterByUniqueId(string uniqueId) =>
        await _mwFifth.Characters.FindOneAsync(x => x.UniqueId == uniqueId)
        ?? throw new ResourceNotFoundException();
    
    public async Task<CharacterAndRevisions> GetRevisions(string? characterId, Account account, bool isAdmin)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.Id == characterId)
                .Select(x => new { x.AccountId, x.UniqueId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        var character = await GetCharacterByUniqueId(reference.UniqueId);

        // If not admin, fail
        if (!isAdmin && account.AccountId == reference.AccountId)
            throw new ResourceForbiddenException();

        var revisions = await _mwFifth.CharacterRevisions
            .Find(x => x.UniqueId == reference.UniqueId)
            .ToListAsync();

        var result = new List<CharacterRevision>();

        // Start with the first character revision
        var revision = revisions.First(x => x.PreviousId == null);
        result.Add(revision);

        // Add each character revision in order
        while (true)
        {
            revision = revisions.FirstOrDefault(x => x.PreviousId == revision.Id);
            if (revision == null) break;
            result.Add(revision);
        }

        // Add any drafts 
        result.AddRange(revisions.Except(result));

        return new CharacterAndRevisions(character, result.ToArray());
    }

    public async Task Approve(string characterId)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.Id == characterId)
                .Select(x => new
                {
                    x.State,
                    x.UniqueId,
                    x.CharacterName,
                    Moonstone = x.GiftMoonstone + x.SkillMoonstone
                })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        await _mwFifth.Characters
            .UpdateOneAsync(x => x.UniqueId == reference.UniqueId,
                Builders<Character>.Update
                    .Set(x => x.CharacterName, reference.CharacterName)
                    .Set(x => x.UsedMoonstone, reference.Moonstone));

        // Mark all (hopefully only one) Live characters are Archive
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.UniqueId == reference.UniqueId && x.State == CharacterState.Live,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Archived)
                    .Set(x => x.ArchivedOn, DateTime.UtcNow));

        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.Id == characterId,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Live)
                    .Set(x => x.ApprovedOn, DateTime.UtcNow));
    }

    public async Task Reject(string characterId)
    {
        var reference =
            await _mwFifth.CharacterRevisions.AsQueryable()
                .Where(x => x.Id == characterId)
                .Select(x => new { x.State, x.UniqueId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        // Mark all (hopefully only one) Live characters are Archive
        await _mwFifth.CharacterRevisions
            .UpdateOneAsync(x => x.UniqueId == reference.UniqueId && x.State == CharacterState.Review,
                Builders<CharacterRevision>.Update
                    .Set(x => x.State, CharacterState.Draft));
    }

    public async Task<CharacterAndRevision> Get(string characterId, Account account, bool isAdmin)
    {
        var revision = await _mwFifth.CharacterRevisions.FindOneAsync(x => x.Id == characterId)
                       ?? throw new ResourceNotFoundException();

        var character = await GetCharacterByUniqueId(revision.UniqueId);

        if (!isAdmin && character.AccountId != account.AccountId)
            throw new ResourceNotFoundException("Character does not belong to Account");

        return new CharacterAndRevision(character, revision);
    }

    public async Task<CharacterAndRevision> GetDraft(string? characterId, Account account, bool isAdmin)
    {
        var reference = await _mwFifth.CharacterRevisions.AsQueryable()
                            .Where(x => x.Id == characterId)
                            .Select(x => new { x.AccountId, x.UniqueId })
                            .FirstOrDefaultAsync()
                        ?? throw new ResourceNotFoundException("CharacterId was not found");

        // If not admin, fail
        if (!isAdmin && account.AccountId != reference.AccountId)
            throw new ResourceNotFoundException("Character does not belong to Account");

        var character = await GetCharacterByUniqueId(reference.UniqueId);

        var revisions = await _mwFifth.CharacterRevisions
            .Find(x => x.UniqueId == reference.UniqueId && x.State != CharacterState.Archived)
            .ToListAsync();

        // If we already have a draft, return that
        var revision = revisions.FirstOrDefault(x => x.State == CharacterState.Draft);
        if (revision != null)
            return new CharacterAndRevision(character, revision);

        // If character is in review, return it if admin, fail if not
        revision = revisions.FirstOrDefault(x => x.State == CharacterState.Review);
        if (isAdmin && revision != null)
            return new CharacterAndRevision(character, revision);

        if (revision != null)
            throw new BadRequestException("Cannot modify character in review");

        revision = revisions.FirstOrDefault(x => x.State == CharacterState.Live)
                   ?? throw new ResourceNotFoundException();

        // Create new draft
        revision.PreviousId = revision.Id;
        revision.Id = ObjectId.GenerateNewId().ToString();
        revision.State = CharacterState.Draft;
        await _mwFifth.CharacterRevisions.InsertOneAsync(revision);
        return new CharacterAndRevision(character, revision);
    }

    public async Task Save(CharacterRevision revision, Account account, bool isAdmin)
    {
        var saved = await Get(revision.Id, account, isAdmin);

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
        var live = await GetLiveByUniqueId(revision.UniqueId);
        revision.ChangeSummary =
            live != null
                ? CharacterRevision.BuildChangeSummary(live, revision)
                : new Dictionary<string, ChangeSummary>();

        if (saved.Revision.State is not CharacterState.Review && revision.State is CharacterState.Review)
            revision.SubmittedOn = DateTime.UtcNow;

        revision.UnlockedAllSpells = saved.Revision.UnlockedAllSpells;
        revision.CreatedOn = saved.Revision.CreatedOn;
        revision.UniqueId = saved.Revision.UniqueId;

        await _mwFifth.CharacterRevisions.ReplaceOneAsync(x => x.Id == revision.Id, revision);
    }

    public async Task<CharacterAndRevision> GetNew(Account account)
    {
        var now = DateTime.UtcNow;
        var character = new Character()
        {
            UniqueId = ObjectId.GenerateNewId().ToString(),
            AccountId = account.AccountId,
            CreatedOn = now,
        };

        var revision = new CharacterRevision
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AccountId = account.AccountId,
            State = CharacterState.Draft,
            UniqueId = character.UniqueId,
            CreatedOn = now
        };

        await _mwFifth.Characters.InsertOneAsync(character);
        await _mwFifth.CharacterRevisions.InsertOneAsync(revision);

        return new CharacterAndRevision(character, revision);
    }

    public async Task Delete(string characterId, Account account, bool isAdmin)
    {
        var character = await Get(characterId, account, isAdmin)
                        ?? throw new ResourceNotFoundException();

        // User cannot change the state to Live
        if (character.Revision.State is not CharacterState.Draft)
            throw new BadReadException("Character must be in draft");

        await _mwFifth.CharacterRevisions.DeleteOneAsync(x => x.Id == characterId);
    }

    public async Task<CharacterAndRevision> GetLatest(string characterId, Account account, bool isAdmin)
    {
        var reference = await _mwFifth.CharacterRevisions.AsQueryable()
            .Where(x => x.Id == characterId)
            .Select(x => new { x.AccountId, x.UniqueId })
            .FirstOrDefaultAsync();

        // If not admin, fail
        if (!isAdmin && account.AccountId == reference.AccountId)
            throw new ResourceNotFoundException();

        var revisions = await _mwFifth.CharacterRevisions
            .Find(x => x.UniqueId == reference.UniqueId && x.State != CharacterState.Archived)
            .ToListAsync();

        var character = await GetCharacterByUniqueId(reference.UniqueId);

        var revision =
            revisions.FirstOrDefault(x => x.State is CharacterState.Draft or CharacterState.Review)
            ?? revisions.FirstOrDefault(x => x.State == CharacterState.Live)
            ?? throw new ResourceNotFoundException();

        return new CharacterAndRevision(character, revision);
    }
}