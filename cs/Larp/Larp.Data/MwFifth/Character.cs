using Larp.Common;

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

    public string Name { get; set; } = default!;
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

    public string Name { get; set; } = default!;
    public int Rank { get; set; }
    public string Title => Rank == 0 ? Name : $"{Name} {Rank}";

    public void Deconstruct(out string name, out int rank)
    {
        name = Name;
        rank = Rank;
    }
}

public class ChangeSummary
{
    public ChangeSummary()
    {
    }

    public ChangeSummary(string? old, string? @new) :
        this(new[] { old }, new[] { @new })
    {
    }

    public ChangeSummary(string?[] old, string?[] @new)
    {
        Old = old;
        New = @new;
    }

    public string?[] Old { get; set; } = Array.Empty<string>();
    public string?[] New { get; set; } = Array.Empty<string>();
}

[PublicAPI]
public class CharacterAttendance
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string UniqueId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = default!;

    public string[] Components { get; set; } = Array.Empty<string>();

    public int AttendanceMoonstone { get; set; }
    public int CleanupMoonstone { get; set; }
    public int WaystonesMoonstone { get; set; }
}

[PublicAPI]
public record CharacterAndRevision(Character Character, CharacterRevision Revision);

[PublicAPI]
public record CharacterAndRevisions(Character Character, CharacterRevision[] CharacterRevisions);

[PublicAPI]
public class Character
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string UniqueId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public DateTimeOffset CreatedOn { get; set; }

    public int Moonstone { get; set; }

    public int UsedMoonstone { get; set; }

    public CharacterAttendance[] Attendances { get; set; } = Array.Empty<CharacterAttendance>();

    public string? CharacterName { get; set; }
}

[PublicAPI]
public class CharacterRevision
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string UniqueId { get; set; } = default!;

    public CharacterState State { get; set; } = CharacterState.Draft;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PreviousId { get; set; }

    public Dictionary<string, ChangeSummary>? ChangeSummary { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? SubmittedOn { get; set; }
    public DateTimeOffset? ApprovedOn { get; set; }
    public DateTimeOffset? ArchivedOn { get; set; }

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

    public int StartingLevel => NoHistory ? 5 : 6;
    public int GiftMoonstone { get; set; }
    public int SkillMoonstone { get; set; }

    public static int Triangle(int level)
    {
        return Math.Max(0, level * (level + 1) / 2);
    }

    public CharacterSummary ToSummary(GameState gameState) =>
        new(
            Id,
            CharacterName ?? "Unnamed",
            GameState.GameName,
            AccountId,
            HomeChapter ?? "Undefined",
            $"{gameState.HomeChapters.FirstOrDefault(x => x.Name == HomeChapter)?.Title ?? "No Home Chapter"}, {Occupation ?? "No Occupation"}, {gameState.Religions.FirstOrDefault(x => x.Name == Religion)?.Title ?? "Not Religious"}",
            Level,
            State);

    private static HashSet<string> _skipProperties = new()
        { nameof(ChangeSummary), nameof(State), nameof(Id), nameof(PreviousId), nameof(AccountId) };

    private record ChangeMap(string[] Added, string[] Removed);

    private static bool Summarize<T>(IEnumerable<T> oldList, IEnumerable<T> newList, Func<T, string> transformer,
        out string[] oldItems, out string[] newItems)
    {
        oldItems = oldList.Select(transformer).OrderBy(x => x).ToArray();
        newItems = newList.Select(transformer).OrderBy(x => x).ToArray();
        return !oldItems.SequenceEqual(newItems);
    }

    public static Dictionary<string, ChangeSummary> BuildChangeSummary(CharacterRevision? oldCharacter,
        CharacterRevision? newCharacter)
    {
        var result = new Dictionary<string, ChangeSummary>();

        if (oldCharacter == null || newCharacter == null)
            return result;

        foreach (var property in typeof(CharacterRevision).GetProperties())
        {
            if (_skipProperties.Contains(property.Name)) continue;
            var oldValue = property.GetValue(oldCharacter);
            var newValue = property.GetValue(newCharacter);

            if (newValue is CharacterSkill[] newSkills && oldValue is CharacterSkill[] oldSkills)
            {
                if (Summarize(oldSkills, newSkills, x => x.Title, out var oldItems, out var newItems))
                    result.Add(property.Name, new ChangeSummary(oldItems, newItems));
            }
            else if (newValue is CharacterVantage[] newVantages && oldValue is CharacterVantage[] oldVantages)
            {
                if (Summarize(newVantages, oldVantages, x => x.Title, out var oldItems, out var newItems))
                    result.Add(property.Name, new ChangeSummary(oldItems, newItems));
            }
            else if (((oldValue == null) != (newValue == null)) || oldValue?.Equals(newValue) == false)
            {
                result.Add(property.Name, new ChangeSummary(oldValue?.ToString(), newValue?.ToString()));
            }
        }

        return result;
    }
}