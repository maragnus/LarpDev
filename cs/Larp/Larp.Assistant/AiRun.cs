namespace Larp.Assistant;

public record AiRun(bool Exists, string ThreadId, string RunId, RunStatus RunStatus, AiMessage[] Messages)
{
    public static AiRun Empty { get; } =
        new(false, string.Empty, string.Empty, RunStatus.Failed, Array.Empty<AiMessage>());
}