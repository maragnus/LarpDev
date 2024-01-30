using Larp.Assistant;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api/clarify")]
public interface IClarifyService
{
    [ApiGet("assistant/{runId}"), ApiAuthenticated()]
    Task<AiRun> UpdateRun(string runId);

    [ApiGet("assistant"), ApiAuthenticated()]
    Task<AiRun> Resume();
    
    [ApiPut("assistant"), ApiAuthenticated()]
    Task<AiRun> StartNew(string message);
    
    [ApiPost("assistant"), ApiAuthenticated()]
    Task<AiRun> Continue(string message);
    
    [ApiDelete("assistant"), ApiAuthenticated()]
    Task Delete();

    [ApiGet("assistants"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<AiConversation[]> GetConversations();
}