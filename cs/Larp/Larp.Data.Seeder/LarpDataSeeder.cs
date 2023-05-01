using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Larp.Data.Mongo;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Larp.Data.Seeder;

public class LarpDataSeeder
{
    private const string LarpResourceNamePrefix = "Larp.Data.Seeder.";

    // ReSharper disable once InconsistentNaming
    private const string Mw5eResourceNamePrefix = LarpResourceNamePrefix + "Mw5e";
    private readonly ISystemClock _clock;

    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public static readonly JsonDocumentOptions JsonDocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };

    private readonly LarpContext _larpContext;
    private readonly ILogger<LarpDataSeeder> _logger;

    public LarpDataSeeder(LarpContext larpContext, ILogger<LarpDataSeeder> logger, ISystemClock clock)
    {
        _larpContext = larpContext;
        _logger = logger;
        _clock = clock;
    }

    public async Task SeedIfNeeded()
    {
        var gameCount = await _larpContext.Games.CountDocumentsAsync(FilterDefinition<Game>.Empty);

        if (gameCount != 0)
        {
            _logger.LogDebug("Seeding skipped because {GameCount} games are present", gameCount);
            return;
        }

        await Seed();
    }

    public async Task Reseed()
    {
        _logger.LogInformation("Removing larp data from database");

        await _larpContext.Games.DeleteManyAsync(_ => true);
        await _larpContext.GameStates.DeleteManyAsync(_ => true);
        
        await Seed();
    }
    
    private async Task Seed()
    {
        _logger.LogInformation("Seeding larp data into database");

        var gameState = new GameState
        {
            Name = GameState.GameName,
            LastUpdated = _clock.UtcNow.ToString("O"),
            Revision = Guid.NewGuid().ToString("N")
        };

        await ImportData(gameState, () => gameState.Advantages);
        await ImportData(gameState, () => gameState.Disadvantages);
        await ImportData(gameState, () => gameState.Gifts);
        await ImportData(gameState, () => gameState.HomeChapters);
        await ImportData(gameState, () => gameState.Occupations);
        await ImportData(gameState, () => gameState.Religions);
        await ImportData(gameState, () => gameState.Skills);
        await ImportData(gameState, () => gameState.Spells);

        await _larpContext.MwFifthGame.SetGameState(gameState);
        
        await ImportCommonData();
    }

    private async Task ImportCommonData()
    {
        var json = await GetJsonDocument(LarpResourceNamePrefix + "LarpData");
        await ImportGames(json.RootElement.GetProperty("games"));
        //await ImportEvents(json.RootElement.GetProperty("events"));
    }

    private async Task ImportGames(JsonElement json)
    {
        var games = Import<Game>(json);
        await _larpContext.Games.InsertManyAsync(games, new InsertManyOptions());
    }

    // ReSharper disable once UnusedMember.Local
    private async Task ImportEvents(JsonElement json)
    {
        var gameIds = (await _larpContext.Games.Find(FilterDefinition<Game>.Empty).ToListAsync())
            .ToDictionary(x => x.Name, x => x.Id);

        var events = Import<Event>(json).ToList();

        foreach (var ev in events)
            ev.GameId = gameIds[ev.GameId];

        await _larpContext.Events.InsertManyAsync(events, new InsertManyOptions());
    }

    private async Task<JsonDocument> GetJsonDocument(string name)
    {
        var fileName = name.Contains('.') ? name : (Mw5eResourceNamePrefix + name);
        await using var jsonFile =
            Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName + ".json")
            ?? throw new ApplicationException($"Embedded resource \"{fileName}.json\" could not be accessed");
        return await JsonDocument.ParseAsync(jsonFile, JsonDocumentOptions);
    }

    private async Task ImportData<TEntity>(GameState gameState, Expression<Func<TEntity[]>> expression)
        where TEntity : class, new()
    {
        var lambda = (LambdaExpression)expression;
        var operand = (MemberExpression)lambda.Body;
        var name = operand.Member.Name;

        _logger.LogInformation("Seeding {CollectionName}", name);

        var json = await GetJsonDocument(name);
        var items = Import<TEntity>(json.RootElement);
        ((PropertyInfo)operand.Member).SetValue(gameState, items.ToArray());
    }

    private IEnumerable<TEntity> Import<TEntity>(JsonElement json)
        where TEntity : class
    {
        foreach (var j in json.EnumerateArray().Select(element => element.GetRawText()))
        {
            TEntity e;
            try
            {
                e = JsonSerializer.Deserialize<TEntity>(j, JsonSerializerOptions)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "JSON: {Json}", j);
                throw;
            }

            yield return e;
        }
    }
}