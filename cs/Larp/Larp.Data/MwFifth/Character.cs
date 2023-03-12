namespace Larp.Data.MwFifth;

[PublicAPI]
public enum SkillPurchase {
    Free,
    Occupation,
    Purchased,
    Bestowed
}

[PublicAPI]
public class CharacterSkill
{
    public string Name { get; set; } = null!;
    public int Rank { get; set; }
    public SkillPurchase Type { get; set; }
    public int? Purchases { get; set; }
}

[PublicAPI]
public class CharacterVantage
{
    public string Name { get; set; } = null!;
    public int Rank { get; set; }
}

[PublicAPI]
public class Character
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string AccountId { get; set; } = null!;

    public string? CharacterName { get; set; }
    public string? Religions { get; set; }
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
} 

