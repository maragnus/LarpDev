using System.Net.Http.Headers;
using System.Net.Http.Json;
using JetBrains.Annotations;
using Larp.Landing.Shared;
using Microsoft.Extensions.FileProviders;

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
    
    protected async Task<TResult> Get<TResult>(string uri) where TResult : class =>
        (await _httpClient.GetFromJsonAsync<TResult>(uri, LarpJson.Options))!;

    private class DownloadFileInfo : IFileInfo
    {
        private readonly MemoryStream _stream;

        public DownloadFileInfo(MemoryStream stream, string name, int length)
        {
            Name = name;
            Length = length;
            _stream = stream;
        }

        public Stream CreateReadStream() => _stream;

        public bool Exists => true;
        public long Length { get; }
        public string? PhysicalPath => null;
        public string Name { get; }
        public DateTimeOffset LastModified => DateTimeOffset.Now;
        public bool IsDirectory => false;
    }

    protected async Task<IFileInfo> Download(string uri)
    {
        var bytes = await _httpClient.GetByteArrayAsync(uri);
        var stream = new MemoryStream(bytes);
        return new DownloadFileInfo(stream, "download", bytes.Length);
    }
    
    protected async Task Post(string uri)
    {
        var response = await _httpClient.PostAsync(uri, content: new StringContent(""));
        if (!response.IsSuccessStatusCode)
            throw new LandingServiceException(await response.Content.ReadAsStringAsync());
    }
    
    protected async Task Post(string uri, object body)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, body, LarpJson.Options);
        if (!response.IsSuccessStatusCode)
            throw new LandingServiceException(await response.Content.ReadAsStringAsync());
    }

    protected async Task<TResult> PostFile<TResult>(string uri, Stream data, string fileName) where TResult : class
    {
        var content = new MultipartFormDataContent
        {
            { new StreamContent(data), "file", fileName }
        };

        var response = await _httpClient.PostAsync(uri, content);
        if (!response.IsSuccessStatusCode)
            throw new LandingServiceException(await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<TResult>(LarpJson.Options))!;
    }
    
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