namespace Larp.Data;

public enum CharacterState
{
    /// <summary>Previously Live but replaced with a new revision</summary>
    Archived = 0,

    /// <summary>Current character ready for game</summary>
    Live = 1,

    /// <summary>Revisions are waiting for Game Master approval</summary>
    Review = 2,

    /// <summary>Draft of a character revision</summary>
    Draft = 3,

    /// <summary>Character is read-only</summary>
    Retired = 4,
}

// Game agnostic character summary
public record CharacterSummary(
    string Id,
    string Name,
    string GameName,
    string PlayerId,
    string HomeChapter,
    string Summary,
    int Level,
    CharacterState State
);