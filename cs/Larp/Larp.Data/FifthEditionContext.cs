using Larp.Data.Services;
using Larp.Protos.Mystwood5e;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Data;

public class FifthEditionContext
{
    private readonly LarpDataCache _cache;
    public const string PrefixName = "Mw5e";
    private const string GameStateCacheName = PrefixName + "." + nameof(GameState);
    
    public FifthEditionContext(IMongoDatabase database, LarpDataCache cache)
    {
        _cache = cache;
        Characters = database.GetCollection<Character>($"{PrefixName}.{nameof(Characters)}");
        GameStates = database.GetCollection<BsonDocument>($"{PrefixName}.{nameof(GameStates)}");
    }

    public IMongoCollection<BsonDocument> GameStates { get; }

    public IMongoCollection<Character> Characters { get; }

    public async ValueTask<GameState> GetGameState()
    {
        if (_cache.TryGetValue(GameStateCacheName, out GameState gameState)) 
            return gameState;
        
        var bson = await GameStates.Find(FilterDefinition<BsonDocument>.Empty).FirstAsync();
        return _cache.Set(GameStateCacheName, bson.ToMessage<GameState>());
    }

    public async Task SetGameState(GameState gameState)
    {
        _cache.Set(GameStateCacheName, gameState);
        
        await GameStates.ReplaceOneAsync(
            Builders<BsonDocument>.Filter.Empty,
            gameState.ToBsonDocument(),
            new ReplaceOptions { IsUpsert = true});
    }
}