using Larp.Data;
using Larp.Data.MwFifth;

namespace Larp.Landing.Shared;

public class CharacterAccountSummary
{
    public CharacterAccountSummary()
    {
    }
    
    public CharacterAccountSummary(CharacterRevision revision, Account account)
        : this(revision.RevisionId, account.AccountId, revision.State, account.Name, account.EmailList,
            revision.CharacterName, revision.HomeChapter, revision.Occupation, revision.Level, revision.GiftMoonstone + revision.SkillMoonstone)
    {
    }

    public CharacterAccountSummary(string characterId,
        string accountId,
        CharacterState state,
        string? accountName,
        string? accountEmails,
        string? characterName,
        string? homeChapter,
        string? occupation,
        int? level,
        int? moonstone)
    {
        CharacterId = characterId;
        AccountId = accountId;
        State = state;
        AccountName = accountName;
        AccountEmails = accountEmails;
        CharacterName = characterName;
        HomeChapter = homeChapter;
        Occupation = occupation;
        Level = level;
        Moonstone = moonstone;
    }

    public string CharacterId { get; init; } = default!;
    public string AccountId { get; init; } = default!;
    public CharacterState State { get; init; }
    public string? AccountName { get; init; }
    public string? AccountEmails { get; init; }
    public string? CharacterName { get; init; }
    public string? HomeChapter { get; init; }
    public string? Occupation { get; init; }
    public int? Level { get; init; }
    public int? Moonstone { get; set; }
}