using Larp.Assistant;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api/admin/ai")]
public interface IAssistantService
{
    [ApiGet("assistant/{runId}"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<AiRun> UpdateRun(string runId);

    [ApiGet("assistant"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<AiRun> Resume();
    
    [ApiPut("assistant"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<AiRun> StartNew(string message);
    
    [ApiPost("assistant"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<AiRun> Continue(string message);
    
    [ApiDelete("assistant"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task Delete();
}