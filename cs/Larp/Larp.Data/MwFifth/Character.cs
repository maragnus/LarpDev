using System.Text.RegularExpressions;
using Larp.Data.MwFifth;

namespace Larp.Data.MwFifth;

internal static class Utility
{
    public static (string Name, int Rank) SplitNameAndRank(string name)
    {
        var match = Regex.Match(name, @"^(.*) (\d+)$", RegexOptions.Compiled);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value)) 
            : (name, 0);
    }
}

[PublicAPI]
public enum SkillPurchase
{
    Free,
    Occupation,
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
    public string Title => $"{Name} {Rank}";
}

[PublicAPI]
public class Character
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string? CharacterName { get; set; }
    public string[] Religions { get; set; } = Array.Empty<string>();
    public string? Occupation { get; set; }
    public string? Specialty { get; set; }
    public string? Enhancement { get; set; }
    public string? HomeChapter { get; set; }
    public string? PublicStory { get; set; }
    public string? PrivateStory { get; set; }
    public string? Homeland { get; set; }

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

    public CharacterSummary ToSummary() =>
        new CharacterSummary(
            Id,
            CharacterName ?? "Unnamed",
            GameState.GameName,
            AccountId,
            HomeChapter ?? "Undefined",
            $"{Occupation}",
            Level);

    public void ReplaceOccupationalSkills(string[]? occupationalSkills)
    {
        if (occupationalSkills == null)
            return;

        Skills = Skills
            .Where(x => x.Type != SkillPurchase.Occupation)
            .Concat(occupationalSkills.Select(x => CharacterSkill.FromTitle(x, SkillPurchase.Occupation)))
            .ToArray();
    }
}