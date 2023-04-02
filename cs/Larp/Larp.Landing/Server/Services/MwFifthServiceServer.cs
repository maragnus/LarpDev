using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.MwFifth;
using Larp.Landing.Shared.Messages;
using Larp.Landing.Shared.MwFifth;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class MwFifthServiceServer : IMwFifthService
{
    private readonly IUserSession _userSession;
    private readonly MwFifthGameContext _mwFifth;

    public MwFifthServiceServer(LarpContext larpContext, IUserSession userSession)
    {
        _userSession = userSession;
        _mwFifth = larpContext.MwFifthGame;
    }

    public async Task<GameState> GetGameState(string lastRevision)
    {
        var state = await _mwFifth.GetGameState();
        return state.Revision == lastRevision
            ? new GameState { Revision = state.Revision }
            : state;
    }

    public async Task<Character> GetCharacter(string characterId)
    {
        return await _mwFifth.Characters
                   .Find(x => x.Id == characterId && x.AccountId == _userSession.AccountId)
                   .FirstOrDefaultAsync()
               ?? throw new ResourceNotFoundException();
    }

    public async Task<Character> ReviseCharacter(string characterId)
    {
        var revision = await _mwFifth.Characters
            .Find(x =>
                x.State != CharacterState.Live
                && x.PreviousId == characterId
                && x.AccountId == _userSession.AccountId)
            .FirstOrDefaultAsync();
        if (revision != null)
            return revision;

        var character =
            await _mwFifth.Characters
                .Find(x => x.Id == characterId && x.AccountId == _userSession.AccountId)
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException();

        if (character.State != CharacterState.Live)
            throw new BadRequestException("Character must be Live to revise");

        character.PreviousId = character.Id;
        character.Id = ObjectId.GenerateNewId().ToString();
        character.State = CharacterState.UpdateDraft;
        character.UniqueId = character.UniqueId;
        character.CreatedOn = character.CreatedOn;

        await _mwFifth.Characters.InsertOneAsync(character);
        return character;
    }

    public async Task<Character> GetNewCharacter()
    {
        var character = new Character
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AccountId = _userSession.AccountId!,
            State = CharacterState.NewDraft,
            UniqueId = ObjectId.GenerateNewId().ToString(),
            CreatedOn = DateTime.UtcNow
        };

        await _mwFifth.Characters.InsertOneAsync(character);
        
        return character;
    }

    public async Task<StringResult> SaveCharacter(Character character)
    {
        var saved = await _mwFifth.Characters
            .Find(x => x.Id == character.Id)
            .FirstOrDefaultAsync();
        
        var live = saved?.PreviousId != null
            ? await _mwFifth.Characters
                .Find(x => x.Id == character.PreviousId)
                .FirstOrDefaultAsync() : null;
        var isLive = live != null;
        
        if (saved == null)
            throw new BadRequestException("Character does not exist");
        
        if (saved.AccountId != _userSession.AccountId)
            throw new BadRequestException("Character does not belong to Account");
        
        if (saved.AccountId != character.AccountId)
            throw new BadRequestException("Character cannot change owner");

        if (isLive)
        {
            // User cannot change the state inappropriately
            if (character.State is not (CharacterState.UpdateDraft or CharacterState.Review))
                throw new BadRequestException("State may only be UpdateDraft or Review");
            
            character.ChangeSummary = Character.BuildChangeSummary(live, character);
        }
        else
        {
            // User cannot change the state inappropriately
            if (character.State is not (CharacterState.NewDraft or CharacterState.Review))
                throw new BadRequestException("State may only be NewDraft or Review");
            
            character.ChangeSummary = new();
        }

        // User cannot edit character that is live or waiting for review
        if (saved.State is CharacterState.Live or CharacterState.Archived or CharacterState.Review)
            throw new BadRequestException("Character must be in draft to edit");

        if (saved.State is not CharacterState.Review && character.State is CharacterState.Review)
            character.SubmittedOn = DateTime.UtcNow;
        
        character.UnlockedAllSpells = saved.UnlockedAllSpells;
        character.CreatedOn = saved.CreatedOn;
        character.UniqueId = saved.UniqueId;

        await _mwFifth.Characters.ReplaceOneAsync(x => x.Id == character.Id, character);
        return StringResult.Success(character.Id);
    }

    public async Task DeleteCharacter(string characterId)
    {
        var character = await _mwFifth.Characters
            .Find(x => x.Id == characterId)
            .FirstOrDefaultAsync();

        // User cannot change the state to Live
        if (character.State is CharacterState.Live or CharacterState.Review)
            throw new ResourceForbiddenException();

        await _mwFifth.Characters.DeleteOneAsync(x => x.Id == characterId);
    }
}