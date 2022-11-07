using Google.Protobuf;
using Larp.Data.Services;
using Larp.Protos.Mystwood5e;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Larp.Data;

public class FifthEditionContext
{
    private readonly LarpDataCache _cache;
    public const string PrefixName = "Mw5e";
    
    public FifthEditionContext(IMongoDatabase database, LarpDataCache cache)
    {
        _cache = cache;
        Characters = database.GetCollection<Character>($"{PrefixName}.{nameof(Characters)}");
        GameStates = database.GetCollection<BsonDocument>($"{PrefixName}.{nameof(GameStates)}");
    }

    public IMongoCollection<BsonDocument> GameStates { get; set; }

    public IMongoCollection<Character> Characters { get; set; }

    public async ValueTask<GameState> GetGameState()
    {
        if (_cache.TryGetValue(nameof(GameState), out GameState gameState)) 
            return gameState;
        
        var bson = await GameStates.Find(FilterDefinition<BsonDocument>.Empty).FirstAsync();
        gameState = JsonParser.Default.Parse<GameState>(bson.ToJson());
        _cache.Set(nameof(GameState), gameState);
        return gameState;
    }

    public async Task SetGameState(GameState gameState)
    {
        _cache.Set(nameof(GameState), gameState);
        
        var json = JsonFormatter.Default.Format(gameState);
        var bson = BsonDocument.Parse(json);
        
        await GameStates.InsertOneAsync(bson, new InsertOneOptions());
    }
}
