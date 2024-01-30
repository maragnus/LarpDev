using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Threads;

namespace Larp.Assistant.OpenAi;

public class OpenAiAssistant : IAiAssistant
{
    private readonly ILogger<OpenAiAssistant> _logger;
    private readonly OpenAiOptions _options;

    public OpenAiAssistant(IOptions<OpenAiOptions> options, ILogger<OpenAiAssistant> logger)
    {
        _logger = logger;
        _options = options.Value;
    }
    
    private OpenAIClient GetClient() => new(new OpenAIAuthentication(_options.ApiKey, _options.OrganizationId));

    public async Task<AiRun> StartConversation(string assistantId, string message, string? additionalInstructions = null)
    {
        using var api = GetClient();
        var run = await api.ThreadsEndpoint.CreateThreadAndRunAsync(
            new CreateThreadAndRunRequest(assistantId, createThreadRequest: message));
        return await RunResponse(run);
    }
    
    public async Task<AiRun> ContinueConversation(string assistantId, string threadId, string message, string? additionalInstructions = null)
    {
        using var api = new OpenAIClient(new OpenAIAuthentication(_options.ApiKey, _options.OrganizationId));
        await api.ThreadsEndpoint.CreateMessageAsync(threadId, new CreateMessageRequest(message));
        var run = await api.ThreadsEndpoint.CreateRunAsync(threadId, new CreateRunRequest(assistantId));
        return await RunResponse(run);
    }

    public async Task<AiRun> GetConversation(string threadId)
    {
        using var api = new OpenAIClient(new OpenAIAuthentication(_options.ApiKey, _options.OrganizationId));
        var thread = await api.ThreadsEndpoint.RetrieveThreadAsync(threadId);
        var messages = await thread.ListMessagesAsync();
        var runs = await thread.ListRunsAsync();
        var run = runs.Items.FirstOrDefault(run =>
            run.Status is OpenAI.Threads.RunStatus.Cancelling or OpenAI.Threads.RunStatus.Queued
                or OpenAI.Threads.RunStatus.InProgress or OpenAI.Threads.RunStatus.RequiresAction);
        
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Json: {Messages}", JsonSerializer.Serialize(messages));
        
        return BuildAiRun(threadId, run?.Id ?? "", run?.Status ?? OpenAI.Threads.RunStatus.Completed, messages);
    }

    public async Task<AiRun> UpdateRun(string threadId, string runId)
    {
        using var api = new OpenAIClient(new OpenAIAuthentication(_options.ApiKey, _options.OrganizationId));
        var run = await api.ThreadsEndpoint.RetrieveRunAsync(threadId, runId);
        return await RunResponse(run);
    }

    private static async Task<AiRun> RunResponse(RunResponse run) => BuildAiRun(run.ThreadId, run.Id, run.Status, await run.ListMessagesAsync());

    public async Task DeleteConversation(string threadId)
    {
        using var api = new OpenAIClient(new OpenAIAuthentication(_options.ApiKey, _options.OrganizationId));
        await api.ThreadsEndpoint.DeleteThreadAsync(threadId);
    }

    private static AiRun BuildAiRun(string threadId, string runId, OpenAI.Threads.RunStatus status, ListResponse<MessageResponse>? messages) =>
        new(true, threadId, runId, ToRunStatus(status),
            messages?.Items
                .OrderBy(message => message.CreatedAt)
                .Select(message => new AiMessage(message.Id, message.Role == Role.User, message.Role.ToString(), message.PrintContent(), GetAnnotations(message), message.CreatedAt))
                .ToArray()
            ?? Array.Empty<AiMessage>());

    private static RunStatus ToRunStatus(OpenAI.Threads.RunStatus runStatus) => runStatus switch
    {
        OpenAI.Threads.RunStatus.Completed => RunStatus.Completed,
        OpenAI.Threads.RunStatus.Queued => RunStatus.Queued,
        OpenAI.Threads.RunStatus.InProgress => RunStatus.InProgress,
        OpenAI.Threads.RunStatus.Cancelling => RunStatus.InProgress,
        OpenAI.Threads.RunStatus.RequiresAction => RunStatus.InProgress,
        _ => RunStatus.Failed
    };

    private static AiAnnotation[] GetAnnotations(MessageResponse message)
    {
        return message.Content
            .SelectMany(x => x.Text.Annotations)
            .Select(annotation => new AiAnnotation(annotation.Text, annotation.StartIndex, annotation.EndIndex,
                annotation.FileCitation.FileId, annotation.FileCitation.Quote))
            .ToArray();
    }
}