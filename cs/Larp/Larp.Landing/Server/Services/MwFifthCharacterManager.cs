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

    public async Task<Character[]> GetRevisions(string? characterId, Account account, bool isAdmin)
    {
        var reference =
            await _mwFifth.Characters.AsQueryable()
                .Where(x => x.Id == characterId)
                .Select(x => new { x.AccountId, x.UniqueId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        // If not admin, fail
        if (!isAdmin && account.AccountId == reference.AccountId)
            throw new ResourceForbiddenException();

        var revisions = await _mwFifth.Characters
            .Find(x => x.UniqueId == reference.UniqueId)
            .ToListAsync();

        var result = new List<Character>();

        // Start with the first character revision
        var character = revisions.First(x => x.PreviousId == null);
        result.Add(character);
        
        // Add each character revision in order
        while (true)
        {
            character = revisions.FirstOrDefault(x => x.PreviousId == character.Id);
            if (character == null) break;
            result.Add(character);
        }

        // Add any drafts 
        result.AddRange(revisions.Except(result));
        
        return result.ToArray();
    }

    public async Task Approve(string characterId)
    {
        var reference =
            await _mwFifth.Characters.AsQueryable()
                .Where(x => x.Id == characterId)
                .Select(x => new { x.State, x.UniqueId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        // Mark all (hopefully only one) Live characters are Archive
        await _mwFifth.Characters
            .UpdateOneAsync(x => x.UniqueId == reference.UniqueId && x.State == CharacterState.Live,
                Builders<Character>.Update
                    .Set(x => x.State, CharacterState.Archived)
                    .Set(x => x.ArchivedOn, DateTime.UtcNow));

        await _mwFifth.Characters
            .UpdateOneAsync(x => x.Id == characterId,
                Builders<Character>.Update
                    .Set(x => x.State, CharacterState.Live)
                    .Set(x => x.ApprovedOn, DateTime.UtcNow));
    }

    public async Task Reject(string characterId)
    {
        var reference =
            await _mwFifth.Characters.AsQueryable()
                .Where(x => x.Id == characterId)
                .Select(x => new { x.State, x.UniqueId })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (reference.State != CharacterState.Review)
            throw new BadRequestException("Character state must be in Review");

        // Mark all (hopefully only one) Live characters are Archive
        await _mwFifth.Characters
            .UpdateOneAsync(x => x.UniqueId == reference.UniqueId && x.State == CharacterState.Review,
                Builders<Character>.Update
                    .Set(x => x.State, CharacterState.Draft));
    }

    public async Task<Character?> Get(string? characterId, Account account, bool isAdmin)
    {
        if (characterId == null) return null;

        var character = await _mwFifth.Characters.FindOneAsync(x => x.Id == characterId)
                        ?? throw new ResourceNotFoundException();

        if (!isAdmin && character != null && character.AccountId != account.AccountId)
            throw new BadRequestException("Character does not belong to Account");

        return character;
    }

    public async Task<Character> GetDraft(string? characterId, Account account, bool isAdmin)
    {
        var reference = await _mwFifth.Characters.AsQueryable()
            .Where(x => x.Id == characterId)
            .Select(x => new { x.AccountId, x.UniqueId })
            .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException("CharacterId was not found");

        // If not admin, fail
        if (!isAdmin && account.AccountId != reference.AccountId)
            throw new ResourceNotFoundException();

        var revisions = await _mwFifth.Characters
            .Find(x => x.UniqueId == reference.UniqueId && x.State != CharacterState.Archived)
            .ToListAsync();

        // If we already have a draft, return that
        var character = revisions.FirstOrDefault(x => x.State == CharacterState.Draft);
        if (character != null)
            return character;

        // If character is in review, return it if admin, fail if not
        character = revisions.FirstOrDefault(x => x.State == CharacterState.Review);
        if (isAdmin && character != null)
            return character;
        if (character != null)
            throw new BadRequestException("Cannot modify character in review");

        character = revisions.FirstOrDefault(x => x.State == CharacterState.Live)
                    ?? throw new ResourceNotFoundException();

        character.PreviousId = character.Id;
        character.Id = ObjectId.GenerateNewId().ToString();
        character.State = CharacterState.Draft;
        await _mwFifth.Characters.InsertOneAsync(character);
        return character;
    }

    public async Task Save(Character character, Account account, bool isAdmin)
    {
        var saved = await Get(character.Id, account, isAdmin);
        var live = await Get(character.PreviousId, account, isAdmin);
        var isLive = live != null;

        if (saved == null)
            throw new BadRequestException("Character does not exist");

        if (saved.AccountId != character.AccountId)
            throw new BadRequestException("Character cannot change owner");

        if (!isAdmin && saved.State != CharacterState.Draft)
            throw new BadRequestException("Character must be in draft to save changes");

        if (isAdmin && saved.State is not CharacterState.Draft and not CharacterState.Review)
            throw new BadRequestException("Character must be in draft or review to save changes");

        // User cannot change the state inappropriately
        if (character.State is not (CharacterState.Draft or CharacterState.Review))
            throw new BadRequestException("State may only be Draft or Review");

        if (isLive)
            character.ChangeSummary = Character.BuildChangeSummary(live, character);
        else
            character.ChangeSummary = new Dictionary<string, ChangeSummary>();

        if (saved.State is not CharacterState.Review && character.State is CharacterState.Review)
            character.SubmittedOn = DateTime.UtcNow;

        character.UnlockedAllSpells = saved.UnlockedAllSpells;
        character.CreatedOn = saved.CreatedOn;
        character.UniqueId = saved.UniqueId;

        await _mwFifth.Characters.ReplaceOneAsync(x => x.Id == character.Id, character);
    }

    public async Task<Character> GetNew(Account account)
    {
        var character = new Character
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AccountId = account.AccountId,
            State = CharacterState.Draft,
            UniqueId = ObjectId.GenerateNewId().ToString(),
            CreatedOn = DateTime.UtcNow
        };

        await _mwFifth.Characters.InsertOneAsync(character);

        return character;
    }

    public async Task Delete(string characterId, Account account, bool isAdmin)
    {
        var character = await Get(characterId, account, isAdmin)
                        ?? throw new ResourceNotFoundException();

        // User cannot change the state to Live
        if (character.State is not CharacterState.Draft)
            throw new BadReadException("Character must be in draft");

        await _mwFifth.Characters.DeleteOneAsync(x => x.Id == characterId);
    }

    public async Task<Character> GetLatest(string characterId, Account account, bool isAdmin)
    {
        var reference = await _mwFifth.Characters.AsQueryable()
            .Where(x => x.Id == characterId)
            .Select(x => new { x.AccountId, x.UniqueId })
            .FirstOrDefaultAsync();

        // If not admin, fail
        if (!isAdmin && account.AccountId == reference.AccountId)
            throw new ResourceNotFoundException();

        var revisions = await _mwFifth.Characters
            .Find(x => x.UniqueId == reference.UniqueId && x.State != CharacterState.Archived)
            .ToListAsync();

        return revisions.FirstOrDefault(x => x.State == CharacterState.Draft)
               ?? revisions.FirstOrDefault(x => x.State == CharacterState.Review)
               ?? revisions.FirstOrDefault(x => x.State == CharacterState.Live)
               ?? throw new ResourceNotFoundException();
    }
}