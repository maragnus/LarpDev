using System.Net.Http.Headers;
using KiloTx.Restful;

namespace Larp.Landing.Client.RestClient;

public class HttpClientFactory : IHttpClientFactory
{
    private readonly HttpClient _httpClient;

    public HttpClientFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void SetAuthenticationToken(string? token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Basic", token);
    }

    public HttpClient CreateHttpClient(Type serviceType) => _httpClient;
}