using System.Runtime.CompilerServices;
using Larp.Common;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Larp.Landing.Shared.MwFifth;

public enum CharacterBuilderMode
{
    NewCharacter,
    EditCharacter
}

public class CharacterBuilder
{
    private DependencyManager<CharacterBuilder> DependencyManager { get; }

    private readonly CharacterBuilderMode _mode;
    private readonly ILogger _logger;

    public Action? StateChanged { get; set; }

    #region GameState

    public Character Character { get; }
    public GameState GameState { get; }

    public Spell[] AllWisdomSpells { get; }
    public Spell[] AllBardicSpells { get; }
    public Spell[] AllDivineSpells { get; }
    public Spell[] AllOccupationalSpells { get; }
    public Dictionary<string, HomeChapter> AllHomeChapters { get; }
    public Religion[] AllReligions { get; set; }
    public Dictionary<string, Gift> AllGifts { get; }

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation))]
    public string[] AllSpecialties { get; private set; } = Array.Empty<string>();

    [DependsOn(nameof(PopulateHomelands), nameof(HomeChapter))]
    public string[] AvailableHomelands { get; private set; } = Array.Empty<string>();

    [DependsOn(nameof(PopulateOccupations), nameof(HomeChapter))]
    public Dictionary<string, Occupation> AvailableOccupations { get; private set; } = new();

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation), nameof(Wisdom))]
    public Spell[] OccupationalSpells { get; private set; } = Array.Empty<Spell>();

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation))]
    public CharacterSkill[] OccupationalSkills { get; private set; } = Array.Empty<CharacterSkill>();

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation))]
    public SkillChoice[] OccupationalSkillsChoices { get; private set; } = Array.Empty<SkillChoice>();

    #endregion

    public CharacterBuilder(Character character, GameState gameState, CharacterBuilderMode mode, ILogger logger)
    {
        _mode = mode;
        _logger = logger;
        Character = character;
        GameState = gameState;
        DependencyManager = new DependencyManager<CharacterBuilder>(logger);

        AllWisdomSpells = GameState.Spells.Where(x => x.IsGiftOfWisdom).ToArray();
        AllBardicSpells = GameState.Spells.Where(x => x.IsBardic).ToArray();
        AllDivineSpells = GameState.Spells.Where(x => x.IsDivine).ToArray();
        AllOccupationalSpells = GameState.Spells.Where(x => x.IsOccupational).ToArray();

        AllHomeChapters = GameState.HomeChapters.ToDictionary(x => x.Name);
        AllGifts = GameState.Gifts.ToDictionary(x => x.Name);

        AllReligions = GameState.Religions;

        DependencyManager.UpdateAll(this);
    }

    private bool HasSkill(string skillName) => Skills.Contains(skillName);

    #region Character

    public string? HomeChapter
    {
        get => Character.HomeChapter;
        set => Set(c => c.HomeChapter = value);
    }

    public string? Homeland
    {
        get => Character.Homeland;
        set => Set(c => c.Homeland = value);
    }

    public string? Occupation
    {
        get => Character.Occupation;
        set => Set(x => x.Occupation = value);
    }

    public string? Specialty
    {
        get => Character.Specialty;
        set => Set(x => x.Specialty = value);
    }

    public string? Religion
    {
        get => Character.Religion;
        set => Set(x => x.Religion = value);
    }

    public bool NoHistory
    {
        get => Character.NoHistory;
        set => Set(x => x.NoHistory = value);
    }

    public bool NoAdvantages
    {
        get => Character.NoAdvantages;
        set => Set(x => x.NoAdvantages = value);
    }

    public string? PublicHistory
    {
        get => Character.PublicHistory;
        set => Set(x => x.PublicHistory = value);
    }

    public string? PrivateHistory
    {
        get => Character.PrivateHistory;
        set => Set(x => x.PrivateHistory = value);
    }

    private string[] _chosenSpells = Array.Empty<string>();

    public string[] ChosenSpells
    {
        get => _chosenSpells;
        set
        {
            _chosenSpells = value;
            DependencyManager.Update(this, nameof(ChosenSpells));
        }
    }

    public int Level => Courage + Dexterity + Empathy + Passion + Prowess + Wisdom;

    public int Courage
    {
        get => Character.Courage;
        set => Set(x => x.Courage = value);
    }

    public int Dexterity
    {
        get => Character.Dexterity;
        set => Set(x => x.Dexterity = value);
    }

    public int Empathy
    {
        get => Character.Empathy;
        set => Set(x => x.Empathy = value);
    }

    public int Passion
    {
        get => Character.Passion;
        set => Set(x => x.Passion = value);
    }

    public int Prowess
    {
        get => Character.Prowess;
        set => Set(x => x.Prowess = value);
    }

    public int Wisdom
    {
        get => Character.Wisdom;
        set => Set(x => x.Wisdom = value);
    }

    public string[] OccupationalChosenSkills
    {
        get => Character.Skills
            .Where(x => x.Type == SkillPurchase.OccupationChoice)
            .Select(x => x.Name)
            .ToArray();

        set
        {
            Character.Skills = Character.Skills
                .Where(x => x.Type != SkillPurchase.OccupationChoice)
                .Concat(value.Select(x => CharacterSkill.FromTitle(x, SkillPurchase.OccupationChoice)))
                .ToArray();
            DependencyManager.Update(this, nameof(OccupationalChosenSkills));
        }
    }

    public CharacterSkill[] PurchasedSkills
    {
        get => Character.Skills
            .Where(x => x.Type == SkillPurchase.Purchased)
            .ToArray();

        set
        {
            Character.Skills = Character.Skills
                .Where(x => x.Type != SkillPurchase.Purchased)
                .Concat(value)
                .ToArray();
            DependencyManager.Update(this, nameof(PurchasedSkills));
        }
    }

    public CharacterVantage[] Advantages
    {
        get => Character.Advantages;
        set => Set(x => x.Advantages = value);
    }

    public CharacterVantage[] Disadvantages
    {
        get => Character.Disadvantages;
        set => Set(x => x.Disadvantages = value);
    }

    public HomeChapter? GetHomeChapter() => GameState.HomeChapters.FirstOrDefault(x => x.Name == Character.HomeChapter);

    public Occupation? GetOccupation() => GameState.Occupations.FirstOrDefault(x => x.Name == Character.Occupation);

    #endregion

    #region Validation

    [DependsOn(nameof(PopulateSkills), nameof(Occupation), nameof(OccupationalChosenSkills), nameof(PurchasedSkills))]
    public HashSet<string> Skills { get; private set; } = new();

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom))]
    public bool HasWisdomSpells { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation))]
    public bool HasBardicSpells { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation))]
    public bool HasDivineSpells { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation))]
    public bool HasOccupationalSpells { get; private set; }

    public bool IsHomeChapterValid => !string.IsNullOrEmpty(Character.HomeChapter);
    public bool IsHomelandValid => !string.IsNullOrWhiteSpace(Character.Homeland);

    [DependsOn(nameof(PopulateIsOccupationValid), nameof(Occupation), nameof(OccupationalChosenSkills),
        nameof(Specialty))]
    public bool IsOccupationValid { get; private set; }

    [DependsOn(nameof(PopulateIsOccupationValid), nameof(Occupation), nameof(OccupationalSkillsChoices))]
    public bool IsChosenSkillsValid { get; private set; }
    
    public bool IsGiftsValid => Level is >= 5 and <= 6;
    public bool IsReligionValid => !string.IsNullOrEmpty(Character.Religion);

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation), nameof(ChosenSpells))]
    public bool IsSpellsValid { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation))]
    public bool HasSpells { get; private set; }

    [DependsOn(nameof(PopulateVantages), nameof(NoAdvantages), nameof(Advantages), nameof(Disadvantages))]
    public bool IsVantagesValid { get; private set; }

    [DependsOn(nameof(PopulateHistory), nameof(NoHistory), nameof(PublicHistory), nameof(PrivateHistory))]
    public bool IsHistoryValid { get; private set; }

    [DependsOn(nameof(PopulateAbilities), nameof(Courage), nameof(Dexterity), nameof(Empathy), nameof(Passion),
        nameof(Prowess), nameof(Wisdom))]
    public string[] Abilities { get; private set; } = Array.Empty<string>();

    [DependsOn(nameof(PopulateAbilities), nameof(Courage), nameof(Dexterity), nameof(Empathy), nameof(Passion),
        nameof(Prowess), nameof(Wisdom))]
    public CharacterProperty[] Properties { get; private set; } = Array.Empty<CharacterProperty>();

    #endregion

    #region Populate

    private void Set(Action<Character> set, [CallerMemberName] string memberName = null!)
    {
        set.Invoke(Character);
        DependencyManager.Update(this, memberName);
        StateChanged?.Invoke();
    }

    private void PopulateAbilities()
    {
        var abilities = new List<Ability>();
        var properties = new Dictionary<string, string>();

        if (Level == 0)
        {
            Abilities = Array.Empty<string>();
            return;
        }

        void Append(string giftName, int giftLevel)
        {
            if (giftLevel <= 0) return;

            // Abilities are accumulative of gift levels
            var a = AllGifts[giftName].Ranks[0..(giftLevel - 1)]
                .SelectMany(x => x.Abilities);
            abilities.AddRange(a);

            // Properties are only from current gift level
            var gift = AllGifts[giftName];
            var rank = gift.Ranks[giftLevel - 1];
            var p = gift.Properties.Select((name, index) =>
                new KeyValuePair<string, string>(name, rank.Properties[index]));
            properties.AddRange(p);
        }

        Append(nameof(Courage), Courage);
        Append(nameof(Dexterity), Dexterity);
        Append(nameof(Empathy), Empathy);
        Append(nameof(Passion), Passion);
        Append(nameof(Prowess), Prowess);
        Append(nameof(Wisdom), Wisdom);

        Abilities = abilities
            .OrderByDescending(x => x.Rank)
            .DistinctBy(x => x.Name)
            .Select(x => x.Title)
            .Order()
            .ToArray();


        Properties = properties.Select(x => new CharacterProperty(x)).ToArray();
    }

    private void PopulateOccupations()
    {
        var type = Character.IsYouth ? OccupationType.Youth : OccupationType.Basic;

        if (_mode == CharacterBuilderMode.NewCharacter)
        {
            AvailableOccupations =
                GameState.Occupations
                    .Where(o => o.Type == type
                                && (o.Chapters.Length == 0 || o.Chapters.Contains(HomeChapter)))
                    .ToDictionary(x => x.Name);
        }
        else
        {
            AvailableOccupations =
                GameState.Occupations
                    .Where(o => o.Chapters.Length == 0 || o.Chapters.Contains(HomeChapter))
                    .ToDictionary(x => x.Name);
        }
    }

    private void PopulateHomelands()
    {
        AvailableHomelands = GetHomeChapter()?.Homelands ?? Array.Empty<string>();
    }

    private void PopulateOccupationalDependencies()
    {
        var occupation = GetOccupation();

        OccupationalSkills = occupation?.Skills
                                 .Select(skillTitle => CharacterSkill.FromTitle(skillTitle, SkillPurchase.Occupation))
                                 ?.ToArray()
                             ?? Array.Empty<CharacterSkill>();

        OccupationalSkillsChoices = occupation?.Choices
                                    ?? Array.Empty<SkillChoice>();

        AllSpecialties = occupation?.Specialties ?? Array.Empty<string>();
        if (!AllSpecialties.Contains(Character.Specialty))
            Character.Specialty = null;
    }

    private void PopulateSkills()
    {
        Skills = OccupationalSkills.Select(x => x.Name)
            .Concat(OccupationalChosenSkills)
            .Concat(PurchasedSkills.Select(x => x.Name))
            .ToHashSet();
    }

    private void PopulateIsOccupationValid()
    {
        if (GetOccupation() == null)
        {
            _logger.LogInformation("Occupation not selected");
            IsOccupationValid = false;
            return;
        }

        if (OccupationalSkillsChoices.Length > 0)
        {
            var chosenSkills = Character.Skills
                .Where(x => x.Type == SkillPurchase.OccupationChoice)
                .Select(x => x.Title)
                .ToHashSet();

            var chosenSkillsAreValue =
                OccupationalSkillsChoices.All(choice =>
                    choice.Count == choice.Choices.Count(skillName => chosenSkills.Contains(skillName)));
            if (!chosenSkillsAreValue)
            {
                _logger.LogInformation("Occupational skills choices not selected {Json}, {Choices}",
                    chosenSkills.ToJson(), OccupationalSkillsChoices.ToJson());
                IsOccupationValid = false;
                IsChosenSkillsValid = false;
                return;
            }

            IsChosenSkillsValid = true;
        }

        if (AllSpecialties.Length > 0)
        {
            if (string.IsNullOrEmpty(Specialty))
            {
                _logger.LogInformation("Occupational specialty not selected");
                IsOccupationValid = false;
                return;
            }
        }

        IsOccupationValid = true;
    }

    private void PopulateSpells()
    {
        HasWisdomSpells = Character.Wisdom > 0;
        HasBardicSpells = HasWisdomSpells && HasSkill("Bardic Voice");
        HasDivineSpells = HasWisdomSpells && HasSkill("Divine Spells");
        HasOccupationalSpells = HasWisdomSpells && HasSkill("Occupational Spells");
        OccupationalSpells = AllOccupationalSpells.Where(x => x.Category == Character.Occupation).ToArray();

        var spells = new HashSet<string>();
        if (HasWisdomSpells) spells.AddRange(ChosenSpells);
        if (HasBardicSpells) spells.AddRange(AllBardicSpells.Select(x => x.Name));
        if (HasDivineSpells) spells.AddRange(AllDivineSpells.Select(x => x.Name));
        if (HasOccupationalSpells) spells.AddRange(OccupationalSpells.Select(x => x.Name));
        Character.Spells = spells.ToArray();

        IsSpellsValid = ChosenSpells.Length == Character.Wisdom;
        HasSpells = HasWisdomSpells;
    }

    private void PopulateVantages()
    {
        IsVantagesValid =
            NoAdvantages
            || (
                Disadvantages.Length > 0
                && Advantages.Sum(x => x.Rank) <= Disadvantages.Sum(x => x.Rank));
    }

    private void PopulateHistory()
    {
        IsHistoryValid = NoHistory | !string.IsNullOrWhiteSpace(PublicHistory);
    }

    #endregion
}