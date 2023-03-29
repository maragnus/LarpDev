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

        await _mwFifth.Characters.InsertOneAsync(character);
        return character;
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

        if (oldCharacter != null)
        {
            // User cannot change the state inappropriately
            if (character.PreviousId == null &&
                character.State is not (CharacterState.NewDraft or CharacterState.Review))
                throw new BadRequestException("State may only be NewDraft or Review");

            if (character.PreviousId != null &&
                character.State is not (CharacterState.UpdateDraft or CharacterState.Review))
                throw new BadRequestException("State may only be UpdateDraft or Review");

            // Properties are admin-only
            if (character.UnlockedAllSpells != oldCharacter.UnlockedAllSpells
                || character.PreviousId != oldCharacter.PreviousId)
                throw new BadRequestException("Cannot update restricted fields");

            // User can only edit their own characters
            if (oldCharacter.AccountId != _userSession.AccountId)
                throw new BadRequestException("Cannot change owner");

            // User cannot edit character that is live or waiting for review
            if (oldCharacter.State is not (CharacterState.NewDraft or CharacterState.UpdateDraft))
                throw new BadRequestException("Character must be in draft to edit");

            var previousCharacter = await _mwFifth.Characters
                .Find(x => x.Id == character.PreviousId)
                .FirstOrDefaultAsync();

            character.ChangeSummary = Character.BuildChangeSummary(previousCharacter, character);

            await _mwFifth.Characters.ReplaceOneAsync(x => x.Id == character.Id, character);
            return StringResult.Success(character.Id);
        }

        // Property is admin-only
        if (character.UnlockedAllSpells || character.PreviousId != null || character.ChangeSummary != null)
            throw new BadRequestException();

        // User cannot change the state inappropriately
        if (character.State is not (CharacterState.NewDraft or CharacterState.Review))
            throw new ResourceForbiddenException();

        character.AccountId = _userSession.AccountId!;

        await _mwFifth.Characters.InsertOneAsync(character);
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