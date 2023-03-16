using Larp.Data.MwFifth;

namespace Larp.Landing.Shared.MwFifth;

public class CharacterSkillComparer : IEqualityComparer<CharacterSkill> {
    public bool Equals(CharacterSkill? x, CharacterSkill? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Name == y.Name && x.Rank == y.Rank;
    }

    public int GetHashCode(CharacterSkill obj)
    {
        return HashCode.Combine(obj.Name, obj.Rank);
    }
}

public class CharacterValidator
{
    private readonly GameState _gameState;

    private static readonly CharacterSkillComparer _characterSkillComparer = new();
    
    public CharacterValidator(GameState gameState)
    {
        _gameState = gameState;
    }

    public bool IsHomeChapterValid(Character character) =>
        _gameState.HomeChapters.Any(x => x.Name == character.HomeChapter);

    public bool IsHomelandValid(Character character) =>
        !string.IsNullOrWhiteSpace(character.Homeland);
    
    public (bool isSuccess, string reason) IsOccupationValid(Character character)
    {
        // Occupation is invalid
        var occupation = _gameState.Occupations.FirstOrDefault(x => x.Name == character.Occupation);
        if (occupation == null) return (false, "Occupation is required");

        // Specialty is invalid
        if (occupation.Specialties.Length > 0 && occupation.Specialties.All(x => x != character.Specialty))
            return (false, "Specialty is required");

        // Skills match occupation
        var skills = character.Skills
            .Where(x => x.Type == SkillPurchase.Occupation)
            .ToHashSet();
        var occupationIncludedSkills = occupation.Skills
            .Select(x => CharacterSkill.FromTitle(x, SkillPurchase.Occupation))
            .ToHashSet();
        var occupationChosenSkills = occupation.Choices.SelectMany(x => x.Choices)
            .Select(x => CharacterSkill.FromTitle(x, SkillPurchase.Occupation))
            .ToHashSet();
        
        // All skills must be in occupationAllSkills
        if (skills.Except(occupationChosenSkills, _characterSkillComparer).Except(occupationIncludedSkills, _characterSkillComparer).Any())
            return (false, "Occupation skills are not included");

        // All occupationIncludedSkills must be in skills
        if (occupationIncludedSkills.Except(skills, _characterSkillComparer).Any())
            return (false, "Occupation skills are not included");
        
        // Skill choices made correctly
        foreach (var choices in occupation.Choices)
        {
            var skillChoices = choices.Choices
                .Select(x => CharacterSkill.FromTitle(x, SkillPurchase.Occupation))
                .Union(skills)
                .ToHashSet();

            if (skillChoices.Count != choices.Count)
                return (false, "Occupation skill choices are incomplete");
        }

        return (true, "Success");
    }

    public bool IsGiftsValid(Character character)
    {
        return character.Level != 6;
    }

    public bool IsReligionValid(Character character) =>
        character.Religions.Length > 0
        && character.Religions.All(religionName => _gameState.Religions.Any(religion => religion.Name == religionName));

    public bool IsSpellsValid(Character character) =>
        character.Spells.All(spellName => _gameState.Spells.Any(spell => spell.Name == spellName));

    public bool IsVantagesValid(Character character) =>
        character.Advantages.Sum(x => x.Rank) <= character.Disadvantages.Sum(x=>x.Rank);

    public bool IsHistoryValid(Character character) =>
        !string.IsNullOrWhiteSpace(character.PublicStory) || !string.IsNullOrWhiteSpace(character.PrivateStory);
}