using Larp.Assistant;
using Larp.Assistant.OpenAi;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class ClarifyService : IClarifyService
{
    private readonly IUserSession _userSession;
    private readonly OpenAiOptions _aiOptions;
    private readonly IUserSessionManager _userSessionManager;
    private readonly IAiAssistant _aiAssistant;
    private readonly ILogger<ClarifyService> _logger;
    private readonly LarpContext _db;

    public ClarifyService(IUserSession userSession, IOptions<OpenAiOptions> aiOptions, LarpContext db,
        IUserSessionManager userSessionManager, IAiAssistant aiAssistant, ILogger<ClarifyService> logger)
    {
        _userSession = userSession;
        _aiOptions = aiOptions.Value;
        _userSessionManager = userSessionManager;
        _aiAssistant = aiAssistant;
        _logger = logger;
        _db = db;
    }

    public async Task<AiRun> UpdateRun(string runId)
    {
        try
        {
            return await _aiAssistant.UpdateRun(GetThreadId(), runId);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Failed to update thread {ThreadId} run {RunId}", GetThreadId(), runId);
            return AiRun.Empty;
        }
    }

    public async Task<AiRun> Resume()
    {
        if (_userSession.AccountId is null || _userSession.Account is null) 
            throw new ResourceUnauthorizedException();

        var threadId = _userSession.Account.ClarifyThreadId;
        if (string.IsNullOrEmpty(threadId)) return AiRun.Empty;

        try
        {
            _logger.LogInformation("Resuming {ThreadId}", threadId);
            return await _aiAssistant.GetConversation(GetThreadId());
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Thread not found {ThreadId}", threadId);
            return AiRun.Empty;
        }
    }

    public async Task<AiRun> StartNew(string message)
    {
        if (_userSession.AccountId is null || _userSession.Account is null) 
            throw new ResourceUnauthorizedException();
        
        var threadId = _userSession.Account.ClarifyThreadId;
        if (!string.IsNullOrEmpty(threadId))
        {
            try
            {
                await _aiAssistant.DeleteConversation(threadId);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Previous ThreadId {ThreadId} could not be deleted", threadId);
            }
        }
        
        var run = await _aiAssistant.StartConversation(_aiOptions.PlayerAssistantId!, message);
        await _userSessionManager.UpdateUserAccount(_userSession.AccountId!, update => update.Set(account => account.ClarifyThreadId, run.ThreadId));
        return run;
    }

    public async Task<AiRun> Continue(string message)
    {
        if (_userSession.AccountId is null || _userSession.Account is null) 
            throw new ResourceUnauthorizedException();
        
        var threadId = _userSession.Account.ClarifyThreadId;
        if (string.IsNullOrEmpty(threadId))
            return await StartNew(message);
        return await _aiAssistant.ContinueConversation(_aiOptions.PlayerAssistantId!, GetThreadId(), message);
    }

    public async Task Delete()
    {
        try
        {
            await _aiAssistant.DeleteConversation(GetThreadId());
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Failed to delete {ThreadId}", GetThreadId());
        }
    }

    public async Task<AiConversation[]> GetConversations()
    {
        var accounts = await _db.Accounts.Find(a => a.ClarifyThreadId != null && a.ClarifyThreadId != "").ToListAsync();
        var results = new List<AiConversation>();
        foreach (var account in accounts)
        {
            var run = await _aiAssistant.GetConversation(account.ClarifyThreadId!);
            if (!run.Exists) continue;
            results.Add(new AiConversation(true, account.AccountId, account.Name ?? "Name Not Set", run));
        }

        return results.ToArray();
    }

    private string GetThreadId()
    {
        if (_userSession.AccountId is null || _userSession.Account is null) 
            throw new ResourceUnauthorizedException();

        return _userSession.Account.ClarifyThreadId 
            ?? throw new ResourceNotFoundException();
    }
}