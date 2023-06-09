using Larp.Common;

namespace Larp.Data.MwFifth;

public record NameRank(string Name, int Rank);

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
    public string Name { get; set; } = default!;
    public int Rank { get; set; }
    public string Title => Rank == 0 ? Name : $"{Name} {Rank}";
    public SkillPurchase Type { get; set; }
    public int? Purchases { get; set; }

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
}

[PublicAPI]
public class CharacterVantage
{
    public string Name { get; set; } = default!;
    public int Rank { get; set; }
    public string Title => Rank == 0 ? Name : $"{Name} {Rank}";

    public static CharacterVantage FromTitle(string title)
    {
        var (name, rank) = Utility.SplitNameAndRank(title);

        return new CharacterVantage()
        {
            Name = name,
            Rank = rank
        };
    }

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

public class MoonstoneInfo
{
    public MoonstoneInfo()
    {
    }

    public MoonstoneInfo(int moonstoneTotal, int moonstoneUsed)
    {
        Total = moonstoneTotal;
        Used = moonstoneUsed;
        Available = moonstoneTotal - moonstoneUsed;
    }

    public int Total { get; set; }
    public int Used { get; set; }
    public int Available { get; set; }
}

[PublicAPI]
public record CharacterAndRevision(Character Character, CharacterRevision Revision, MoonstoneInfo Moonstone);

[PublicAPI]
public record CharacterAndRevisions(Character Character, CharacterRevision[] CharacterRevisions,
    MoonstoneInfo Moonstone);

[PublicAPI]
public class Character
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string CharacterId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public DateTimeOffset CreatedOn { get; set; }

    public CharacterState State { get; set; }

    public int Moonstone { get; set; }

    public int UsedMoonstone { get; set; }

    public string? CharacterName { get; set; }

    public int? ImportId { get; set; }

    public int? ImportedMoonstone { get; set; }

    public string? PreregistrationNotes { get; set; }

    public string? PreregistrationRevisionNotes { get; set; }
    public static string? RevisionReviewerNotes { get; set; }
}

[PublicAPI]
public class CharacterRevision
{
    private static HashSet<string> _skipProperties = new()
        { nameof(ChangeSummary), nameof(State), nameof(RevisionId), nameof(PreviousRevisionId), nameof(AccountId) };

    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string RevisionId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string CharacterId { get; set; } = default!;

    public CharacterState State { get; set; } = CharacterState.Draft;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PreviousRevisionId { get; set; }

    public Dictionary<string, ChangeSummary>? ChangeSummary { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? SubmittedOn { get; set; }
    public DateTimeOffset? ApprovedOn { get; set; }
    public DateTimeOffset? ArchivedOn { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ApprovedBy { get; set; }

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
    public string? ChosenElement { get; set; }
    public string? UnusualFeatures { get; set; }
    public string? Cures { get; set; }
    public string? Documents { get; set; }
    public string? Notes { get; set; }
    public string? PreregistrationNotes { get; set; }

    public bool UnlockedAllSpells { get; set; }
    public bool NoHistory { get; set; }
    public bool NoAdvantages { get; set; }
    public bool NoOccupation { get; set; }

    public int StartingLevel => NoHistory ? 5 : 6;
    public int GiftMoonstone { get; set; }
    public int SkillMoonstone { get; set; }
    public int TotalMoonstone => GiftMoonstone + SkillMoonstone;

    public string? RevisionReviewerNotes { get; set; }
    public string? RevisionPlayerNotes { get; set; }

    public static int Triangle(int level)
    {
        return Math.Max(0, level * (level + 1) / 2);
    }

    public CharacterSummary ToSummary(GameState gameState) =>
        new(
            RevisionId,
            CharacterName ?? "Unnamed",
            GameState.GameName,
            AccountId,
            HomeChapter ?? "Undefined",
            $"{gameState.HomeChapters.FirstOrDefault(x => x.Name == HomeChapter)?.Title ?? "No Home Chapter"}, {Occupation ?? "No Occupation"}, {gameState.Religions.FirstOrDefault(x => x.Name == Religion)?.Title ?? "Not Religious"}",
            Level,
            State);

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

            if (newValue is string[] newStrings && oldValue is string[] oldStrings)
            {
                if (Summarize(oldStrings, newStrings, x => x, out var oldItems, out var newItems))
                    result.Add(property.Name, new ChangeSummary(oldItems, newItems));
            }
            else if (newValue is CharacterSkill[] newSkills && oldValue is CharacterSkill[] oldSkills)
            {
                foreach (var type in Enum.GetValues<SkillPurchase>())
                {
                    var oldSkillsFiltered = oldSkills.Where(x => x.Type == type);
                    var newSkillsFiltered = newSkills.Where(x => x.Type == type);
                    if (Summarize(oldSkillsFiltered, newSkillsFiltered, x => x.Title, out var oldItems,
                            out var newItems))
                        result.Add($"{property.Name} ({type})", new ChangeSummary(oldItems, newItems));
                }
            }
            else if (newValue is CharacterVantage[] newVantages && oldValue is CharacterVantage[] oldVantages)
            {
                if (Summarize(oldVantages, newVantages, x => x.Title, out var oldItems, out var newItems))
                    result.Add(property.Name, new ChangeSummary(oldItems, newItems));
            }
            else if (((oldValue == null) != (newValue == null)) || oldValue?.Equals(newValue) == false)
            {
                result.Add(property.Name, new ChangeSummary(oldValue?.ToString(), newValue?.ToString()));
            }
        }

        return result;
    }

    public NameRank[] ConsolidatedSkills()
    {
        return Skills
            .GroupBy(x => x.Name)
            .Select(group => new NameRank(group.Key, group.Sum(skill => skill.Rank)))
            .ToArray();
    }

    private record ChangeMap(string[] Added, string[] Removed);
}