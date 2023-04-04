using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Data.Mongo;

public class MwFifthGameContext
{
    private const string PrefixName = "Mw5e";
    private const string GameStateCacheName = PrefixName + "." + nameof(GameState);
    private readonly LarpDataCache _cache;
    private readonly IMongoCollection<BsonDocument> _gameStates;

    public MwFifthGameContext(IMongoDatabase database, LarpDataCache cache)
    {
        _cache = cache;
        Characters = database.GetCollection<Character>($"{PrefixName}.{nameof(Character)}");
        CharacterRevisions = database.GetCollection<CharacterRevision>($"{PrefixName}.{nameof(CharacterRevisions)}");
        _gameStates = database.GetCollection<BsonDocument>(nameof(LarpContext.GameStates));
    }

    public IMongoCollection<Character> Characters { get; }
    public IMongoCollection<CharacterRevision> CharacterRevisions { get; }

    public async ValueTask<GameState> GetGameState()
    {
        if (_cache.TryGetValue(GameStateCacheName, out GameState gameState))
            return gameState;

        var bson = await _gameStates.
            Find(x => x["Name"] == GameState.GameName)
            .FirstAsync();
        
        bson.Remove("_id");
        var json = bson.ToJson();
        return _cache.Set(GameStateCacheName, System.Text.Json.JsonSerializer.Deserialize<GameState>(json)!);
    }

    public async Task SetGameState(GameState gameState)
    {
        _cache.Set(GameStateCacheName, gameState);

        await _gameStates.ReplaceOneAsync(
            Builders<BsonDocument>.Filter.Empty,
            gameState.ToBsonDocument(),
            new ReplaceOptions { IsUpsert = true });
    }
}