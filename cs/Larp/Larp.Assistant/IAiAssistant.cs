namespace Larp.Assistant;

public interface IAiAssistant
{
    Task<AiRun> StartConversation(string assistantId, string message, string? additionalInstructions = null);
    Task<AiRun> ContinueConversation(string assistantId, string threadId, string message, string? additionalInstructions = null);
    Task<AiRun> GetConversation(string threadId);
    Task<AiRun> UpdateRun(string threadId, string runId);
    Task DeleteConversation(string threadId);
}