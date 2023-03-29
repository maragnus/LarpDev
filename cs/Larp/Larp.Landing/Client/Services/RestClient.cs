using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Larp.Landing.Shared;

namespace Larp.Landing.Client.Services;

[PublicAPI]
public abstract class RestClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    protected RestClient(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public void SetSessionId(string? sessionId)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(sessionId)
                ? null
                : new AuthenticationHeaderValue("Basic", sessionId);
    }

    protected async Task Delete(string uri) =>
        await _httpClient.DeleteAsync(uri);
    
    protected async Task<TResult> Get<TResult>(string uri) where TResult : new() =>
        (await _httpClient.GetFromJsonAsync<TResult>(uri, LarpJson.Options))!;

    protected async Task<TResult> Post<TResult>(string uri) where TResult : class
    {
        var response = await _httpClient.PostAsync(uri, content: new StringContent(""));
        if (!response.IsSuccessStatusCode)
            throw new LandingServiceException(await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<TResult>(LarpJson.Options))!;
    }

    protected async Task<TResult> Post<TResult>(string uri, object item) where TResult : class
    {
        var response = await _httpClient.PostAsJsonAsync(uri, item, LarpJson.Options);
        if (!response.IsSuccessStatusCode)
            throw new LandingServiceException(await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<TResult>(LarpJson.Options))!;
    }

    protected async Task<TItem[]> GetArray<TItem>(string uri) =>
        await _httpClient.GetFromJsonAsync<TItem[]>(uri, LarpJson.Options) ?? Array.Empty<TItem>();
}