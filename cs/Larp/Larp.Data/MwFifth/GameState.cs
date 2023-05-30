using System.Text.Json.Serialization;

namespace Larp.Data.MwFifth;

[PublicAPI]
public class GameStateBase
{
    public string Name { get; set; } = default!;
    public string LastUpdated { get; set; } = default!;
    public string Revision { get; set; } = default!;
}

[PublicAPI]
public class GameState : GameStateBase
{
    public const string GameName = "mw5e";

    public Gift[] Gifts { get; set; } = Array.Empty<Gift>();
    public SkillDefinition[] Skills { get; set; } = Array.Empty<SkillDefinition>();
    public Occupation[] Occupations { get; set; } = Array.Empty<Occupation>();
    public Vantage[] Advantages { get; set; } = Array.Empty<Vantage>();
    public Vantage[] Disadvantages { get; set; } = Array.Empty<Vantage>();
    public Religion[] Religions { get; set; } = Array.Empty<Religion>();
    public HomeChapter[] HomeChapters { get; set; } = Array.Empty<HomeChapter>();
    public Spell[] Spells { get; set; } = Array.Empty<Spell>();
}

[PublicAPI]
public class Gift
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string[] Properties { get; set; } = Array.Empty<string>();
    public GiftRank[] Ranks { get; set; } = Array.Empty<GiftRank>();
}

[PublicAPI]
public class GiftProperty
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
}

[PublicAPI]
public class GiftPropertyValue
{
    public GiftPropertyValue()
    {
    }

    public GiftPropertyValue(GiftPropertyValue clone) : this(clone.Name, clone.Value)
    {
    }

    public GiftPropertyValue(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;

    public void Deconstruct(out string name, out string value)
    {
        name = Name;
        value = Value;
    }
}

[PublicAPI]
public class GiftRank
{
    public int Rank { get; set; }
    public string[] Properties { get; set; } = Array.Empty<string>();
    public Ability[] Abilities { get; set; } = Array.Empty<Ability>();
}

[PublicAPI]
public enum SkillClass
{
    Unavailable,
    Free,
    Minor,
    Standard,
    Major
}

[PublicAPI]
public enum SkillPurchasable
{
    Unavailable,
    Once,
    Multiple
}

[PublicAPI]
public class SkillDefinition
{
    public string Name { get; set; } = default!;

    public string Title { get; set; } = default!;

    public SkillClass Class { get; set; }

    public SkillPurchasable Purchasable { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? RanksPerPurchase { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? CostPerPurchase { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string[]? Iterations { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Description { get; set; } = default!;

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PreregistrationNote { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string[]? Chapters { get; set; }
}

[PublicAPI]
public class Ability
{
    public string Name { get; set; } = default!; // Name without rank
    public int Rank { get; set; } // 0 if there's only one rank, otherwise represents I, II, IV, 1, 2 ,3
    public string Title { get; set; } = default!; // Name and Rank verbatim
}

[PublicAPI]
public enum OccupationType
{
    Basic,
    Youth,
    Advanced,
    Plot,
    Enhancement
}

// Indicates a single skill or a choice of skills
[PublicAPI]
public class SkillChoice
{
    public int Count { get; set; }
    public string[] Choices { get; set; } = Array.Empty<string>();
}

[PublicAPI]
public class Occupation
{
    public string Name { get; set; } = default!;

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string[]? Specialties { get; set; }

    public OccupationType Type { get; set; }

    public string[] Skills { get; set; } = Array.Empty<string>();

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SkillChoice[]? Choices { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Duty { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Livery { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Leadership { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string[]? Chapters { get; set; }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Name.GetHashCode();

    public bool HasSkill(string skillName) => Skills.Contains(skillName);

    public bool IsChapter(string? homeChapter)
    {
        // If this is not chapter-specific, match
        if (Chapters == null || Chapters.Length == 0)
            return true;
        // If it is chapter-specific and the player hasn't selected a chapter, no match
        if (string.IsNullOrEmpty(homeChapter))
            return false;
        // If it is chapter-specific, and the character's chapter matches, match
        return Chapters.Contains(homeChapter);
    }
}

[PublicAPI]
public class Vantage
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int Rank { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Physical { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    [BsonIgnoreIfDefault, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PreregistrationNote { get; set; }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Title.GetHashCode();
}

[PublicAPI]
public class Religion
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Name.GetHashCode();
}

[PublicAPI]
public class HomeChapter
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string[] Homelands { get; set; } = Array.Empty<string>();
    public string Email { get; set; } = default!;
    public bool Retired { get; set; }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Name.GetHashCode();
}

[PublicAPI]
public enum SpellType
{
    Bolt,
    Gesture,
    Spray,
    Voice,
    Storm,
    Room,
    GestureOrVoice
}

[PublicAPI]
public class Spell
{
    public string Name { get; set; } = default!;
    public SpellType Type { get; set; }
    public string Category { get; set; } = default!;
    public string[] Categories { get; set; } = Array.Empty<string>();
    public int Mana { get; set; }
    public string Effect { get; set; } = default!;

    public bool IsBardic => Categories.Contains("Bardic");
    public bool IsDivine => Categories.Any(x => x.StartsWith("Divine"));
    public bool IsGiftOfWisdom => Categories.Contains("Gift of Wisdom");
    public bool IsOccupational => !IsBardic && !IsDivine && !IsGiftOfWisdom;

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Name.GetHashCode();
}