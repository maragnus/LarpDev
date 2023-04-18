namespace KiloTx.Restful;

public interface IHttpClientFactory
{
    HttpClient CreateHttpClient(Type serviceType);
}