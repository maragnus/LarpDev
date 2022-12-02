using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Larp.Protos.Mystwood5e;
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

    private readonly JsonDocumentOptions _jsonDocumentOptions = new()
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

        await ImportCommonData();

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

    private async Task ImportCommonData()
    {
        var json = await GetJsonDocument(LarpResourceNamePrefix + "LarpData");
        await ImportGames(json.RootElement.GetProperty("games"));
        await ImportEvents(json.RootElement.GetProperty("events"));
    }

    private async Task ImportGames(JsonElement json)
    {
        var games = Import<Protos.Game>(json)
            .Select(x => new Data.Game(x))
            .ToList();

        await _larpContext.Games.InsertManyAsync(games, new InsertManyOptions());
    }

    private async Task ImportEvents(JsonElement json)
    {
        var gameIds = (await _larpContext.Games.Find(_ => true).ToListAsync())
            .ToDictionary(x => x.Name, x => x.Id);

        var events = Import<Protos.Event>(json)
            .Select(x => new Data.Event(x))
            .ToList();

        foreach (var ev in events)
            ev.GameId = gameIds[ev.GameId];

        await _larpContext.Events.InsertManyAsync(events, new InsertManyOptions() { });
    }

    private async Task<JsonDocument> GetJsonDocument(string name)
    {
        var fileName = name.Contains('.') ? name : (Mw5eResourceNamePrefix + name);
        await using var jsonFile =
            Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName + ".json")
            ?? throw new ApplicationException($"Embedded resource \"{fileName}.json\" could not be accessed");
        return await JsonDocument.ParseAsync(jsonFile, _jsonDocumentOptions);
    }

    private async Task ImportData<TEntity>(Expression<Func<RepeatedField<TEntity>>> expression)
        where TEntity : IMessage<TEntity>, new()
    {
        var lambda = (LambdaExpression)expression;
        var operand = (MemberExpression)lambda.Body;
        var name = operand.Member.Name;

        _logger.LogInformation("Seeding {CollectionName}", name);

        var entities = expression.Compile().Invoke();
        entities.AddRange(Import<TEntity>((await GetJsonDocument(name)).RootElement));
    }

    private IEnumerable<TEntity> Import<TEntity>(JsonElement json)
        where TEntity : IMessage<TEntity>, new()
    {
        foreach (var element in json.EnumerateArray())
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