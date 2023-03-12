namespace Larp.Data.MwFifth;

[PublicAPI]
public class GameStateBase
{
    public string Name { get; set; } = null!;
    public string LastUpdated { get; set; } = null!;
    public string Revision { get; set; } = null!;
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
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string[] Properties { get; set; } = Array.Empty<string>();
    public GiftRank[] Ranks { get; set; } = Array.Empty<GiftRank>();
}

[PublicAPI]
public class GiftProperty
{
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
}

[PublicAPI]
public class GiftPropertyValue
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
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
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public SkillClass Class { get; set; }
    public SkillPurchasable Purchasable { get; set; } 
    public int? RanksPerPurchase { get; set; }
    public int? CostPerPurchase { get; set; }
    public string[] Iterations { get; set; } = Array.Empty<string>();
}

[PublicAPI]
public class Ability
{
    public string Name { get; set; }  = null!; // Name without rank
    public int Rank { get; set; } // 0 if there's only one rank, otherwise represents I, II, IV, 1, 2 ,3
    public string Title { get; set; } = null!; // Name and Rank verbatim
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
    public string Name { get; set; } = null!;
    public string[] Specialties { get; set; } = Array.Empty<string>();
    public string Type { get; set; } = null!;
    public string[] Skills { get; set; } = Array.Empty<string>();
    public SkillChoice[] Choices { get; set; } = Array.Empty<SkillChoice>();
    public string? Duty { get; set; }
    public string? Livery { get; set; }
}

[PublicAPI]
public class Vantage
{
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int Rank { get; set; }
    public bool Physical { get; set; }
}

[PublicAPI]
public class Religion
{
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
}

[PublicAPI]
public class HomeChapter
{
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
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
    public string Name { get; set; } = null!;
    public SpellType Type { get; set; }
    public string Category { get; set; } = null!;
    public int Mana { get; set; }
    public string Effect { get; set; } = null!;
}