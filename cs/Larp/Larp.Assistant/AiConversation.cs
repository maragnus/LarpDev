namespace Larp.Assistant;

public record AiConversation(bool Exists, string AccountId, string AccountName, AiRun Run)
{
    public static AiConversation Empty { get; } =
        new(false, string.Empty, string.Empty, AiRun.Empty);
}