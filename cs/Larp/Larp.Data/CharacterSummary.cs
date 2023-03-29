namespace Larp.Data;

public enum CharacterState
{
    /// <summary>New Character, still in edit mode</summary>
    NewDraft,
    
    /// <summary>Existing Character, still in edit mode</summary>
    UpdateDraft,
    
    /// <summary>Revisions are waiting for Game Master approval</summary>
    Review,
    
    /// <summary>Current character ready for game</summary>
    Live
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