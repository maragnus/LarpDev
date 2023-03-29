using System.Text.Json;
using Larp.Common;
using Larp.Landing.Shared;
using MongoDB.Bson.Serialization.Serializers;

namespace Larp.Data.MwFifth;

public enum AgeGroup
{
    /// <summary>Ages 10-13</summary>
    PreTeen,

    /// <summary>Ages 14-16</summary>
    Youth,

    /// <summary>Ages 16-17</summary>
    YoungAdult,

    /// <summary>Ages 18+</summary>
    Adult
}

[PublicAPI]
public enum SkillPurchase
{
    Free,
    Occupation,
    OccupationChoice,
    Purchased,
    Bestowed
}

[PublicAPI]
public class CharacterSkill
{
    public static CharacterSkill FromTitle(string title, SkillPurchase type)
    {
        var (name, rank) = Utility.SplitNameAndRank(title);

        return new CharacterSkill()
        {
            Name = name,
            Rank = rank,
            Type = type
        };
    }

    public string Name { get; set; } = null!;
    public int Rank { get; set; }
    public string Title => Rank == 0 ? Name : $"{Name} {Rank}";
    public SkillPurchase Type { get; set; }
    public int? Purchases { get; set; }
}

[PublicAPI]
public class CharacterVantage
{
    public static CharacterVantage FromTitle(string title)
    {
        var (name, rank) = Utility.SplitNameAndRank(title);

        return new CharacterVantage()
        {
            Name = name,
            Rank = rank
        };
    }

    public string Name { get; set; } = null!;
    public int Rank { get; set; }  
    public string Title => Rank == 0 ? Name : $"{Name} {Rank}";
}

[PublicAPI]
public class Character
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    public string AccountId { get; set; } = null!;

    public CharacterState State { get; set; } = CharacterState.NewDraft;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string? PreviousId { get; set; }

    public Dictionary<string, string>? ChangeSummary { get; set; }

    public string? CharacterName { get; set; }
    public string? Religion { get; set; }
    public string? Occupation { get; set; }
    public string? Specialty { get; set; }
    public string? Enhancement { get; set; }
    public string? HomeChapter { get; set; }
    public string? PublicHistory { get; set; }
    public string? PrivateHistory { get; set; }
    public string? Homeland { get; set; }
    public AgeGroup? AgeGroup { get; set; }

    public int StartingMoonstone { get; set; }
    public int SkillTokens { get; set; }

    public int Level => Courage + Dexterity + Empathy + Passion + Prowess + Wisdom;
    public int Courage { get; set; }
    public int Dexterity { get; set; }
    public int Empathy { get; set; }
    public int Passion { get; set; }
    public int Prowess { get; set; }
    public int Wisdom { get; set; }

    public CharacterSkill[] Skills { get; set; } = Array.Empty<CharacterSkill>();
    public CharacterVantage[] Advantages { get; set; } = Array.Empty<CharacterVantage>();
    public CharacterVantage[] Disadvantages { get; set; } = Array.Empty<CharacterVantage>();
    public string[] Spells { get; set; } = Array.Empty<string>();
    public string[] FlavorTraits { get; set; } = Array.Empty<string>();

    public string? UnusualFeatures { get; set; }
    public string? Cures { get; set; }
    public string? Documents { get; set; }
    public string? Notes { get; set; }

    public bool UnlockedAllSpells { get; set; }
    public bool NoHistory { get; set; }
    public bool NoAdvantages { get; set; }
    public bool NoOccupation { get; set; }

    public CharacterSummary ToSummary(GameState gameState) =>
        new CharacterSummary(
            Id,
            CharacterName ?? "Unnamed",
            GameState.GameName,
            AccountId,
            HomeChapter ?? "Undefined",
            $"{gameState.HomeChapters.FirstOrDefault(x=>x.Name == HomeChapter)?.Title ?? "No Home Chapter"}, {Occupation ?? "No Occupation"}, {gameState.Religions.FirstOrDefault(x=>x.Name == Religion)?.Title ?? "Not Religious"}",
            Level,
            State);

    public void ReplaceOccupationalSkills(string[]? occupationalSkills)
    {
        if (occupationalSkills == null)
            return;

        Skills = Skills
            .Where(x => x.Type != SkillPurchase.Occupation)
            .Concat(occupationalSkills.Select(x => CharacterSkill.FromTitle(x, SkillPurchase.Occupation)))
            .ToArray();
    }

    private static HashSet<string> _skipProperties = new() { nameof(ChangeSummary), nameof(State), nameof(Id),nameof(PreviousId),nameof(AccountId) };
    
    public static Dictionary<string, string> BuildChangeSummary(Character oldCharacter, Character newCharacter)
    {
        var result = new Dictionary<string, string>();
        foreach (var property in typeof(Character).GetProperties())
        {
            if (_skipProperties.Contains(property.Name)) continue;
            var oldValue = property.GetValue(oldCharacter);
            var newValue = property.GetValue(newCharacter);
            if (oldValue?.Equals(newValue) == false)
            {
                result.Add(property.Name, JsonSerializer.Serialize(oldValue, LarpJson.Options));
            }
        }

        return result;
    }
}