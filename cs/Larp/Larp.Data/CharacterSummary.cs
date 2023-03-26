namespace Larp.Data;

// Game agnostic character summary
public record CharacterSummary(
    string Id,
    string Name,
    string GameName,
    string PlayerId,
    string HomeChapter,
    string Summary,
    int Level
);