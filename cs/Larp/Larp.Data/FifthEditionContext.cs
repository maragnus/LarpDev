 using System.Text.Json;
using Larp.Proto.Mystwood5e;
using MongoDB.Driver;

namespace Larp.Data;

public class FifthEditionContext
{
    public FifthEditionContext(IMongoDatabase database)
    {
        Characters =  database.GetCollection<Character>(nameof(Characters));
        Gifts = database.GetCollection<Gift>(nameof(Gifts));
        HomeChapters = database.GetCollection<HomeChapter>(nameof(HomeChapters));
        Occupations = database.GetCollection<Occupation>(nameof(Occupations));
        Religions = database.GetCollection<Religion>(nameof(Religions));
        Skills = database.GetCollection<SkillDefinition>(nameof(Skills));
        Spells = database.GetCollection<Spell>(nameof(Spells));
        Vantages = database.GetCollection<Vantage>(nameof(Vantages));
    }

    public IMongoCollection<Character> Characters { get; set; }
    public IMongoCollection<Gift> Gifts { get; }
    public IMongoCollection<HomeChapter> HomeChapters { get; }
    public IMongoCollection<Occupation> Occupations { get; set; }
    public IMongoCollection<Religion> Religions { get; set; }
    public IMongoCollection<SkillDefinition> Skills { get; }
    public IMongoCollection<Spell> Spells { get; }
    public IMongoCollection<Vantage> Vantages { get; }
}