namespace KiloTx.Restful;


public class RestfulImplementationAttribute : Attribute
{
    public Type Interface { get; }

    protected RestfulImplementationAttribute(Type type)
    {
        Interface = type;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RestfulImplementationAttribute<TInterface> : RestfulImplementationAttribute
{
    public RestfulImplementationAttribute() : base(typeof(TInterface))
    {
    }
}