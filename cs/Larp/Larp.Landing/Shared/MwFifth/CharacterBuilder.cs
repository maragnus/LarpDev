using System.Runtime.CompilerServices;
using Larp.Common;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Larp.Landing.Shared.MwFifth;

public class CharacterBuilder
{
    private readonly ILogger _logger;

    public CharacterBuilder(CharacterAndRevision character, GameState gameState, ILogger logger)
    {
        _logger = logger;
        Character = character.Character;
        Revision = character.Revision;
        GameState = gameState;
        DependencyManager = new DependencyManager<CharacterBuilder>(logger);

        AllWisdomSpells = GameState.Spells.Where(x => x.IsGiftOfWisdom).ToArray();
        AllBardicSpells = GameState.Spells.Where(x => x.IsBardic).ToArray();
        AllDivineSpells = GameState.Spells.Where(x => x.IsDivine).ToArray();
        AllOccupationalSpells = GameState.Spells.Where(x => x.IsOccupational).ToArray();
        AllPurchasableSkills = GameState.Skills.Where(x => x.Purchasable != SkillPurchasable.Unavailable).ToArray();
        AllHomeChapters = GameState.HomeChapters.ToDictionary(x => x.Name);
        AllGifts = GameState.Gifts.ToDictionary(x => x.Name);
        AllReligions = GameState.Religions;
        AllSpells = GameState.Spells
            .GroupBy(x => x.Name) // Some Occupations share skills
            .ToDictionary(x => x.Key, x => x.First());

        _chosenSpells = AllSpells
            .TryFromKeys(Revision.Spells)
            .Where(x => x.IsGiftOfWisdom)
            .Select(x => x.Name)
            .ToArray();

        PopulateOccupations();
        PopulateOccupationalDependencies();
        PopulateSkills();
        DependencyManager.UpdateAll(this);
    }

    private DependencyManager<CharacterBuilder> DependencyManager { get; }

    public Action? StateChanged { get; set; }

    private bool HasSkill(string skillName) => Skills.Contains(skillName);

    #region GameState

    public Character Character { get; }
    public CharacterRevision Revision { get; }
    public GameState GameState { get; }

    public Spell[] AllWisdomSpells { get; }
    public Spell[] AllBardicSpells { get; }
    public Spell[] AllDivineSpells { get; }
    public Spell[] AllOccupationalSpells { get; }
    public Dictionary<string, HomeChapter> AllHomeChapters { get; }
    public Religion[] AllReligions { get; set; }
    public Dictionary<string, Gift> AllGifts { get; }
    public Dictionary<string, Spell> AllSpells { get; }

    [DependsOn(nameof(PopulateOccupations), nameof(HomeChapter), nameof(AgeGroup))]
    public Dictionary<string, Occupation> AvailableOccupations { get; private set; } = new();

    [DependsOn(nameof(PopulateOccupations), nameof(HomeChapter), nameof(AgeGroup))]
    public Dictionary<string, Occupation> AllEnhancements { get; private set; } = new();

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation), nameof(NoOccupation))]
    public string[] AllSpecialties { get; private set; } = Array.Empty<string>();

    [DependsOn(nameof(PopulateHomelands), nameof(HomeChapter))]
    public string[] AvailableHomelands { get; private set; } = Array.Empty<string>();

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation), nameof(NoOccupation), nameof(Wisdom),
        nameof(Enhancement))]
    public Spell[] OccupationalSpells { get; private set; } = Array.Empty<Spell>();

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation), nameof(NoOccupation), nameof(Enhancement))]
    public CharacterSkill[] OccupationalSkills
    {
        get => Revision.Skills
            .Where(x => x.Type == SkillPurchase.Occupation)
            .ToArray();

        set
        {
            ReplaceSkills(SkillPurchase.Occupation, value);
            DependencyManager.Update(this, nameof(OccupationalSkills));
            StateChanged?.Invoke();
        }
    }

    [DependsOn(nameof(PopulateOccupationalDependencies), nameof(Occupation), nameof(NoOccupation), nameof(Enhancement))]
    public SkillChoice[] OccupationalSkillsChoices { get; private set; } = Array.Empty<SkillChoice>();

    public SkillDefinition[] AllPurchasableSkills { get; }

    #endregion

    #region Character

    public string? CharacterName
    {
        get => Revision.CharacterName;
        set => Set(c => c.CharacterName = value);
    }

    public AgeGroup? AgeGroup
    {
        get => Revision.AgeGroup;
        set => Set(c => c.AgeGroup = value);
    }

    public string? HomeChapter
    {
        get => Revision.HomeChapter;
        set => Set(c => c.HomeChapter = value);
    }

    public string? Homeland
    {
        get => Revision.Homeland;
        set => Set(c => c.Homeland = value);
    }

    public string? Occupation
    {
        get => Revision.Occupation;
        set => Set(x => x.Occupation = value);
    }

    public string? Enhancement
    {
        get => Revision.Enhancement;
        set => Set(x => x.Enhancement = value);
    }

    public string? Specialty
    {
        get => Revision.Specialty;
        set => Set(x => x.Specialty = value);
    }

    public string? Religion
    {
        get => Revision.Religion;
        set => Set(x => x.Religion = value);
    }

    public bool NoOccupation
    {
        get => Revision.NoOccupation;
        set => Set(x => x.NoOccupation = value);
    }

    public bool NoHistory
    {
        get => Revision.NoHistory;
        set => Set(x => x.NoHistory = value);
    }

    public bool NoAdvantages
    {
        get => Revision.NoAdvantages;
        set => Set(x => x.NoAdvantages = value);
    }

    public string? PublicHistory
    {
        get => Revision.PublicHistory;
        set => Set(x => x.PublicHistory = value);
    }

    public string? PrivateHistory
    {
        get => Revision.PrivateHistory;
        set => Set(x => x.PrivateHistory = value);
    }


    public string? Documents
    {
        get => Revision.Documents;
        set => Set(x => x.Documents = value);
    }


    public string? Cures
    {
        get => Revision.Cures;
        set => Set(x => x.Cures = value);
    }


    public string? UnusualFeatures
    {
        get => Revision.UnusualFeatures;
        set => Set(x => x.UnusualFeatures = value);
    }

    private string[] _chosenSpells;

    public string[] ChosenSpells
    {
        get => _chosenSpells;
        set
        {
            _chosenSpells = value;
            DependencyManager.Update(this, nameof(ChosenSpells));
            StateChanged?.Invoke();
        }
    }

    public int GiftMoonstone => Revision.GiftMoonstone;

    public int SkillMoonstone => Revision.SkillMoonstone;

    public int Level => Courage + Dexterity + Empathy + Passion + Prowess + Wisdom;

    public int Courage
    {
        get => Revision.Courage;
        set => Set(x => x.Courage = value);
    }

    public int Dexterity
    {
        get => Revision.Dexterity;
        set => Set(x => x.Dexterity = value);
    }

    public int Empathy
    {
        get => Revision.Empathy;
        set => Set(x => x.Empathy = value);
    }

    public int Passion
    {
        get => Revision.Passion;
        set => Set(x => x.Passion = value);
    }

    public int Prowess
    {
        get => Revision.Prowess;
        set => Set(x => x.Prowess = value);
    }

    public int Wisdom
    {
        get => Revision.Wisdom;
        set => Set(x => x.Wisdom = value);
    }


    private void ReplaceSkillsByTitle(SkillPurchase purchaseType, IEnumerable<string> spellTitles) =>
        ReplaceSkills(purchaseType, spellTitles.Select(title => CharacterSkill.FromTitle(title, purchaseType)));

    private void ReplaceSkills(SkillPurchase purchaseType, IEnumerable<CharacterSkill> value)
    {
        Revision.Skills = Revision.Skills
            .Where(x => x.Type != purchaseType)
            .Concat(value)
            .Select(FixSkill)
            .OrderBy(x => x.Name).ThenBy(x => x.Rank)
            .ToArray();
    }

    private CharacterSkill FixSkill(CharacterSkill skill)
    {
        if (skill.Rank != 1) return skill;
        var skillDefinition = GameState.Skills.FirstOrDefault(x => x.Name == skill.Name);
        if (skillDefinition == null) return skill;

        if (skillDefinition.Purchasable is SkillPurchasable.Multiple) return skill;
        skill.Rank = 0;
        return skill;
    }

    public string[] OccupationalChosenSkills
    {
        get => Revision.Skills
            .Where(x => x.Type == SkillPurchase.OccupationChoice)
            .Select(x => x.Title)
            .ToArray();

        set
        {
            ReplaceSkillsByTitle(SkillPurchase.OccupationChoice, value);
            DependencyManager.Update(this, nameof(OccupationalChosenSkills));
            StateChanged?.Invoke();
        }
    }

    public CharacterSkill[] PurchasedSkills
    {
        get => Revision.Skills
            .Where(x => x.Type == SkillPurchase.Purchased)
            .ToArray();

        set
        {
            ReplaceSkills(SkillPurchase.Purchased, value);
            DependencyManager.Update(this, nameof(PurchasedSkills));
            UpdateMoonstone();
            StateChanged?.Invoke();
        }
    }

    public CharacterSkill[] FreeSkills
    {
        get => Revision.Skills
            .Where(x => x.Type == SkillPurchase.Free)
            .ToArray();

        set
        {
            ReplaceSkills(SkillPurchase.Free, value);
            DependencyManager.Update(this, nameof(FreeSkills));
            UpdateMoonstone();
            StateChanged?.Invoke();
        }
    }

    public CharacterVantage[] Advantages
    {
        get => Revision.Advantages;
        set => Set(x => x.Advantages = value);
    }

    public CharacterVantage[] Disadvantages
    {
        get => Revision.Disadvantages;
        set => Set(x => x.Disadvantages = value);
    }

    public HomeChapter? GetHomeChapter() =>
        GameState.HomeChapters.FirstOrDefault(x => x.Name == Revision.HomeChapter);

    public Occupation? GetOccupation() =>
        GameState.Occupations.FirstOrDefault(x => x.Name == Revision.Occupation);

    public Occupation? GetEnhancement() =>
        GameState.Occupations.FirstOrDefault(x => x.Name == Revision.Enhancement);

    public Religion? GetReligion() =>
        AllReligions.FirstOrDefault(x => x.Name == Revision.Religion);

    #endregion

    #region Validation

    [DependsOn(nameof(PopulateSkills), nameof(Occupation), nameof(NoOccupation), nameof(OccupationalChosenSkills),
        nameof(PurchasedSkills))]
    public HashSet<string> Skills { get; private set; } = new();

    public bool HasEnhancements => AvailableOccupations.Count > 0 && Revision.PreviousRevisionId != null;

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom))]
    public bool HasWisdomSpells { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation), nameof(NoOccupation))]
    public bool HasBardicSpells { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation), nameof(NoOccupation))]
    public bool HasDivineSpells { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation), nameof(NoOccupation))]
    public bool HasOccupationalSpells { get; private set; }

    public bool IsNameValid => !string.IsNullOrWhiteSpace(Revision.CharacterName);
    public bool IsAgeGroupValid => Revision.AgeGroup != null;
    public bool IsHomeChapterValid => !string.IsNullOrEmpty(Revision.HomeChapter);
    public bool IsHomelandValid => !string.IsNullOrWhiteSpace(Revision.Homeland);

    [DependsOn(nameof(PopulateIsOccupationValid), nameof(AgeGroup), nameof(Occupation), nameof(NoOccupation),
        nameof(OccupationalChosenSkills),
        nameof(Specialty), nameof(AgeGroup))]
    public bool IsOccupationValid { get; private set; }

    [DependsOn(nameof(PopulateIsOccupationValid), nameof(AgeGroup), nameof(Occupation), nameof(NoOccupation),
        nameof(OccupationalSkillsChoices))]
    public bool IsChosenSkillsValid { get; private set; }

    public bool IsGiftsValid
    {
        get
        {
            if (AgeGroup == Data.MwFifth.AgeGroup.PreTeen)
                return Level == 0;
            if (Revision is { State: CharacterState.Draft, PreviousRevisionId: null })
                return (NoHistory && Level == 5) || (!NoHistory && Level == 6);
            return (NoHistory && Level >= 5) || (!NoHistory && Level >= 6);
        }
    }

    public bool IsReligionValid => !string.IsNullOrEmpty(Revision.Religion);

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation), nameof(NoOccupation),
        nameof(PurchasedSkills), nameof(ChosenSpells))]
    public bool IsSpellsValid { get; private set; }

    [DependsOn(nameof(PopulateSpells), nameof(Wisdom), nameof(Occupation), nameof(NoOccupation))]
    public bool HasSpells { get; private set; }

    [DependsOn(nameof(PopulateVantages), nameof(NoAdvantages), nameof(Advantages), nameof(Disadvantages))]
    public bool IsVantagesValid { get; private set; }

    [DependsOn(nameof(PopulateHistory), nameof(NoHistory), nameof(PublicHistory), nameof(PrivateHistory))]
    public bool IsHistoryValid { get; private set; }

    public bool IsSkillsValid => true;

    public bool HasSkills => Revision.PreviousRevisionId != null;

    public bool HasDocuments => Revision.PreviousRevisionId != null;

    [DependsOn(nameof(PopulateAbilities), nameof(Courage), nameof(Dexterity), nameof(Empathy), nameof(Passion),
        nameof(Prowess), nameof(Wisdom))]
    public string[] Abilities { get; private set; } = Array.Empty<string>();

    [DependsOn(nameof(PopulateAbilities), nameof(Courage), nameof(Dexterity), nameof(Empathy), nameof(Passion),
        nameof(Prowess), nameof(Wisdom))]
    public GiftPropertyValue[] Properties { get; private set; } = Array.Empty<GiftPropertyValue>();

    public bool IsValid =>
        IsHistoryValid && IsOccupationValid && IsGiftsValid && IsSpellsValid && IsHomelandValid && IsReligionValid &&
        IsNameValid && IsVantagesValid && IsAgeGroupValid && IsSpellsValid && IsChosenSkillsValid && IsHomeChapterValid;

    public bool IsNewCharacter => Revision.PreviousRevisionId == null;

    public string? RevisionReviewerNotes
    {
        get => Revision.RevisionReviewerNotes;
        set => Set(x => x.RevisionReviewerNotes = value);
    }

    public string? RevisionPlayerNotes
    {
        get => Revision.RevisionPlayerNotes;
        set => Set(x => x.RevisionPlayerNotes = value);
    }

    public bool ShowNotes => !IsNewCharacter || !string.IsNullOrWhiteSpace(RevisionReviewerNotes);

    public bool HasTalents => Advantages.Any(x => x.Name.Contains("Talent"));

    #endregion

    #region Populate

    private void Set(Action<CharacterRevision> set, [CallerMemberName] string memberName = null!)
    {
        set.Invoke(Revision);
        DependencyManager.Update(this, memberName);
        StateChanged?.Invoke();
    }

    public void UpdateMoonstone()
    {
        Revision.GiftMoonstone = Math.Max(0,
            CharacterRevision.Triangle(Level) - CharacterRevision.Triangle(Revision.StartingLevel));

        var purchasedSkills = PurchasedSkills
            .Select(purchase => (
                PurchaseCount: Math.Max(1, purchase.Purchases ?? 1),
                CostPerPurchase: GameState.Skills.First(x => x.Name == purchase.Name).CostPerPurchase ?? 0))
            .ToList();

        if (purchasedSkills.Count == 0)
        {
            Revision.SkillMoonstone = 0;
            return;
        }

        var purchaseCount = purchasedSkills.Sum(x => x.PurchaseCount);
        var purchaseCountCost = CharacterRevision.Triangle(purchaseCount - 1);
        var purchaseCostSum = purchasedSkills.Sum(x => x.CostPerPurchase * x.PurchaseCount);

        Revision.SkillMoonstone = Math.Max(0, purchaseCountCost + purchaseCostSum - Revision.SkillTokens);
    }

    private void PopulateAbilities()
    {
        _logger.LogInformation("Populating Gift for Level {Level}", Level);

        var abilities = new List<Ability>();
        var properties = new List<GiftPropertyValue>();

        if (Level == 0)
        {
            Abilities = Array.Empty<string>();
            return;
        }

        void Append(string giftName, int giftLevel)
        {
            if (giftLevel <= 0) return;

            // Abilities are accumulative of gift levels
            var a = AllGifts[giftName].Ranks
                .Where(rank => rank.Rank <= giftLevel)
                .SelectMany(x => x.Abilities);
            abilities.AddRange(a);

            // Properties are only from current gift level
            var gift = AllGifts[giftName];
            var rank = gift.Ranks.First(rank => rank.Rank == giftLevel);
            var p = gift.Properties.Select((name, index) =>
                new GiftPropertyValue(name, rank.Properties[index]));
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


        Properties = properties.ToArray();
    }

    private void PopulateOccupations()
    {
        IEnumerable<Occupation> occupations;

        AllEnhancements = GameState.Occupations
            .Where(x => x.Type == OccupationType.Enhancement && x.IsChapter(HomeChapter))
            .ToDictionary(x => x.Name);

        if (Revision.PreviousRevisionId == null)
        {
            switch (Revision.AgeGroup)
            {
                default:
                    occupations = Array.Empty<Occupation>();
                    break;
                case Data.MwFifth.AgeGroup.Youth:
                    occupations = GameState.Occupations
                        .Where(o => o.Type is OccupationType.Youth && o.IsChapter(HomeChapter));
                    break;
                case Data.MwFifth.AgeGroup.Adult:
                case Data.MwFifth.AgeGroup.YoungAdult:
                    occupations = GameState.Occupations
                        .Where(o => o.Type is OccupationType.Basic && o.IsChapter(HomeChapter));
                    break;
            }
        }
        else
        {
            switch (Revision.AgeGroup)
            {
                default:
                    occupations = Array.Empty<Occupation>();
                    break;
                case Data.MwFifth.AgeGroup.Youth:
                    occupations = GameState.Occupations
                        .Where(o => o.Type is OccupationType.Youth && o.IsChapter(HomeChapter));
                    break;
                case Data.MwFifth.AgeGroup.Adult:
                case Data.MwFifth.AgeGroup.YoungAdult:
                    occupations = GameState.Occupations
                        .Where(o =>
                            o.Type is OccupationType.Basic or OccupationType.Advanced or OccupationType.Plot
                            && o.IsChapter(HomeChapter));
                    break;
            }
        }

        AvailableOccupations = occupations
            .ToDictionary(x => x.Name);
    }

    private void PopulateHomelands()
    {
        AvailableHomelands = GetHomeChapter()?.Homelands ?? Array.Empty<string>();
    }

    private void PopulateOccupationalDependencies()
    {
        var occupation = GetOccupation();
        var enhancement = GetEnhancement();

        var occupationSkills =
            occupation?.Skills
                .Select(skillTitle => CharacterSkill.FromTitle(skillTitle, SkillPurchase.Occupation)) ??
            Array.Empty<CharacterSkill>();

        var enhancementSkills =
            enhancement?.Skills
                .Select(skillTitle => CharacterSkill.FromTitle(skillTitle, SkillPurchase.Occupation)) ??
            Array.Empty<CharacterSkill>();

        OccupationalSkills = occupationSkills.Concat(enhancementSkills).ToArray();

        OccupationalSkillsChoices =
            (occupation?.Choices ?? Array.Empty<SkillChoice>())
            .Concat(enhancement?.Choices ?? Array.Empty<SkillChoice>())
            .ToArray();

        AllSpecialties = occupation?.Specialties ?? Array.Empty<string>();
        if (!AllSpecialties.Contains(Revision.Specialty))
            Revision.Specialty = null;
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
        PopulateOccupations();

        if (AgeGroup == Data.MwFifth.AgeGroup.PreTeen)
        {
            IsOccupationValid = true;
            return;
        }

        if (NoOccupation)
        {
            IsOccupationValid = true;
            IsChosenSkillsValid = true;
            return;
        }

        if (Occupation == null)
        {
            _logger.LogDebug("Occupation not selected");
            IsOccupationValid = false;
            IsChosenSkillsValid = true;
            return;
        }

        if (Occupation != null && !AvailableOccupations.ContainsKey(Occupation))
        {
            _logger.LogDebug("Occupation selection is invalid. {Occupation} should be in {AvailableOccupations}",
                Occupation, AvailableOccupations.Keys);
            IsOccupationValid = false;
            IsChosenSkillsValid = true;
            return;
        }

        if (OccupationalSkillsChoices.Length == 0)
            IsChosenSkillsValid = true;

        else if (OccupationalSkillsChoices.Length > 0)
        {
            var chosenSkills = Revision.Skills
                .Where(x => x.Type == SkillPurchase.OccupationChoice)
                .Select(x => x.Title)
                .ToHashSet();

            var chosenSkillsAreValue =
                OccupationalSkillsChoices.All(choice =>
                    choice.Count == choice.Choices.Count(skillName => chosenSkills.Contains(skillName)));
            if (!chosenSkillsAreValue)
            {
                _logger.LogDebug("Occupational skills choices not selected {Json}, {Choices}",
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
                _logger.LogDebug("Occupational specialty not selected");
                IsOccupationValid = false;
                return;
            }
        }

        IsOccupationValid = true;
    }

    private IEnumerable<string> DivineSpells(string category) =>
        AllDivineSpells.Where(spell =>
                spell.Categories.Any(spellCategory => spellCategory == category))
            .Select(spell => spell.Name);

    private void PopulateSpells()
    {
        HasWisdomSpells = Revision.Wisdom > 0;
        HasBardicSpells = HasWisdomSpells && HasSkill("Bardic Voice");
        HasDivineSpells = HasWisdomSpells && HasSkill("Divine Spells");
        HasOccupationalSpells = HasWisdomSpells && HasSkill("Occupational Spells");
        OccupationalSpells = AllOccupationalSpells
            .Where(spell => spell.Categories.Any(category => category == Revision.Occupation))
            .ToArray();

        var spells = new HashSet<string>();
        if (HasWisdomSpells) spells.AddRange(ChosenSpells);
        if (HasBardicSpells) spells.AddRange(AllBardicSpells.Select(x => x.Name));
        if (HasDivineSpells)
        {
            switch (Religion)
            {
                case "justice":
                    spells.AddRange(DivineSpells("Divine Spells of Justice"));
                    break;
                case "mercy":
                    spells.AddRange(DivineSpells("Divine Spells of Mercy"));
                    break;
                case "wild":
                    spells.AddRange(DivineSpells("Divine Spells of the Wild"));
                    break;
                case "all":
                    spells.AddRange(AllDivineSpells.Select(x => x.Name));
                    break;
            }
        }

        if (HasOccupationalSpells) spells.AddRange(OccupationalSpells.Select(x => x.Name));
        Revision.Spells = spells.ToArray();

        IsSpellsValid = IsNewCharacter
            ? ChosenSpells.Length == Revision.Wisdom
            : ChosenSpells.Length >= Revision.Wisdom;
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