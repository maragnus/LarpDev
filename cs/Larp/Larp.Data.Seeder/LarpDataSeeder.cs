using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Larp.Protos.Mystwood5e;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Data.Seeder;

public class LarpDataSeeder
{
    private readonly LarpContext _larpContext;
    private readonly ILogger<LarpDataSeeder> _logger;
    private readonly ISystemClock _clock;

    public LarpDataSeeder(LarpContext larpContext, ILogger<LarpDataSeeder> logger, ISystemClock clock)
    {
        _larpContext = larpContext;
        _logger = logger;
        _clock = clock;
    }

    public async Task Seed()
    {
        var gameCount = await _larpContext.Games.CountDocumentsAsync(FilterDefinition<Game>.Empty);

        if (gameCount != 0)
        {
            _logger.LogDebug("Seeding skipped because {GameCount} games are present", gameCount);
            return;
        }
        
        _logger.LogInformation("Seeding larp data into database");

        var gameState = new GameState
        {
            LastUpdated = _clock.UtcNow.ToString("O"),
            Revision = Guid.NewGuid().ToString("N")
        };
        
        await ImportData(() => gameState.Advantages);
        await ImportData(() => gameState.Disadvantages);
        await ImportData(() => gameState.Gifts);
        await ImportData(() => gameState.HomeChapters);
        await ImportData(() => gameState.Occupations);
        await ImportData(() => gameState.Religions);
        await ImportData(() => gameState.Skills);
        await ImportData(() => gameState.Spells);

        await _larpContext.FifthEdition.SetGameState(gameState);
    }

    
    private readonly JsonDocumentOptions _jsonDocumentOptions = new()
    { 
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };
        
    private async Task<JsonDocument> GetJsonDocument(string name)
    {
        await using var jsonFile = 
            Assembly.GetExecutingAssembly().GetManifestResourceStream(LarpDataResourceNamePrefix + name + ".json") 
            ?? throw new ApplicationException($"Embedded resource \"{LarpDataResourceNamePrefix}{name}.json\" could not be accessed");            
        return await JsonDocument.ParseAsync(jsonFile, _jsonDocumentOptions);
    }
    
    public async Task ImportData<TEntity>(Expression<Func<RepeatedField<TEntity>>> expression)
        where TEntity : IMessage<TEntity>, new()
    {
        var lambda = (LambdaExpression)expression;
        var operand = (MemberExpression)lambda.Body;
        var name = operand.Member.Name;
        
        _logger.LogInformation("Seeding {CollectionName}", name);
        
        var entities = expression.Compile().Invoke();
        entities.AddRange(Import<TEntity>(await GetJsonDocument(name)));
    }
    
    private const string LarpDataResourceNamePrefix = "Larp.Data.Seeder.Mw5e";

    public IEnumerable<TEntity> Import<TEntity>(JsonDocument json)
        where TEntity : IMessage<TEntity>, new()
    {
        foreach (var element in json.RootElement.EnumerateArray())
        {
            var j = element.GetRawText();
            TEntity e;
            try
            {
                e = Google.Protobuf.JsonParser.Default.Parse<TEntity>(j);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, j);
                throw;
            }
            yield return e;

        }
    }
}

