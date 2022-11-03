using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Larp.Data.Seeder;

public class LarpDataSeeder
{
    private readonly LarpContext _larpContext;
    private readonly ILogger<LarpDataSeeder> _logger;

    public LarpDataSeeder(LarpContext larpContext, ILogger<LarpDataSeeder> logger)
    {
        _larpContext = larpContext;
        _logger = logger;
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

        var jsonDocumentOptions = new JsonDocumentOptions { 
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };
        
        async Task<JsonDocument> GetJsonDocument()
        {
            await using var jsonFile = 
                Assembly.GetExecutingAssembly().GetManifestResourceStream(LarpDataResourceName) 
                ?? throw new ApplicationException($"Embedded resource \"{LarpDataResourceName}\" could not be accessed");            
            return await JsonDocument.ParseAsync(jsonFile, jsonDocumentOptions);
        }

        await Import(await GetJsonDocument());
    }

    private const string LarpDataResourceName = "Larp.Data.Seeder.LarpData.json";

    public async Task Import(JsonDocument json)
    {
        async Task Upsert<T>(IMongoCollection<T> mongoCollection, string name)
        {
            if (json.RootElement.TryGetProperty(name, out var c))
            {
                var objs = c.EnumerateArray().Select(x => x.Deserialize<T>()!);
                foreach (var obj in objs)
                {
                    await mongoCollection.InsertOneAsync(obj);
                }
            }
        }
        
        await Upsert(_larpContext.Games, nameof(_larpContext.Games));
        await Upsert(_larpContext.FifthEdition.Gifts, nameof(_larpContext.FifthEdition.Gifts));
        await Upsert(_larpContext.FifthEdition.HomeChapters, nameof(_larpContext.FifthEdition.HomeChapters));
        await Upsert(_larpContext.FifthEdition.Occupations, nameof(_larpContext.FifthEdition.Occupations));
        await Upsert(_larpContext.FifthEdition.Religions, nameof(_larpContext.FifthEdition.Religions));
        await Upsert(_larpContext.FifthEdition.Skills, nameof(_larpContext.FifthEdition.Skills));
        await Upsert(_larpContext.FifthEdition.Spells, nameof(_larpContext.FifthEdition.Spells));
        await Upsert(_larpContext.FifthEdition.Vantages, nameof(_larpContext.FifthEdition.Vantages));
    }
}