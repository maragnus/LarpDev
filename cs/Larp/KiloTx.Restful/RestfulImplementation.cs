namespace KiloTx.Restful;


public class RestfulImplementationAttribute : Attribute
{
    public Type Interface { get; }
    public Type HttpClientFactory { get; }

    protected RestfulImplementationAttribute(Type @interface, Type httpClientFactory)
    {
        Interface = @interface;
        HttpClientFactory = httpClientFactory;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RestfulImplementationAttribute<TInterface, THttpClientFactory> : RestfulImplementationAttribute
    where THttpClientFactory : IHttpClientFactory
{
    public RestfulImplementationAttribute() : base(typeof(TInterface), typeof(THttpClientFactory))
    {
    }
}