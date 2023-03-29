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

    public Task<Character> GetNewCharacter()
    {
        return Task.FromResult(new Character
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AccountId = _userSession.AccountId!,
            State = CharacterState.NewDraft
        });
    }

    public async Task<StringResult> SaveCharacter(Character character)
    {
        var oldCharacter = await _mwFifth.Characters
            .Find(x => x.Id == character.Id)
            .FirstOrDefaultAsync();

        // User cannot change the state to Live
        if (character.State is CharacterState.Live)
            throw new ResourceForbiddenException();

        if (oldCharacter != null)
        {
            // Property is admin-only
            if (character.UnlockedAllSpells != oldCharacter.UnlockedAllSpells)
                throw new ResourceForbiddenException();
            
            // User can only edit their own characters
            if (oldCharacter.AccountId != _userSession.AccountId)
                throw new ResourceForbiddenException();

            // User cannot edit character that is live or waiting for review
            if (oldCharacter.State is CharacterState.Live or CharacterState.Review)
                throw new ResourceForbiddenException();

            await _mwFifth.Characters.ReplaceOneAsync(x => x.Id == character.Id, character);
            return StringResult.Success(character.Id);
        }

        // Property is admin-only
        if (character.UnlockedAllSpells)
            throw new ResourceForbiddenException();
        
        // User cannot change the state
        if (character.State is not CharacterState.NewDraft)
            throw new ResourceForbiddenException();

        character.AccountId = _userSession.AccountId!;
        
        await _mwFifth.Characters.InsertOneAsync(character);
        return StringResult.Success(character.Id);
    }
}