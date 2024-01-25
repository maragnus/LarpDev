namespace Larp.Assistant;

public record AiAnnotation(
    string Label, 
    int StartIndex,
    int EndIndex,
    string FileName,
    string Quote);