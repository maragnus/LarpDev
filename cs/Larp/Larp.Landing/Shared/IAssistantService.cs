using Larp.Assistant;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api/admin/ai")]
public interface IAssistantService
{
    [ApiGet("assistant/{runId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<AiRun> UpdateRun(string runId);

    [ApiGet("assistant"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<AiRun> Resume();
    
    [ApiPut("assistant"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<AiRun> StartNew(string message);
    
    [ApiPost("assistant"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<AiRun> Continue(string message);
    
    [ApiDelete("assistant"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task Delete();

    [ApiGet("assistants"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<AiConversation[]> GetConversations();
}