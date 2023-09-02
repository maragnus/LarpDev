namespace KiloTx.Restful.Server;

public class ResourceUnauthorizedException : Exception
{
    public ResourceUnauthorizedException() : base("User is not authenticated")
    {
    }

    public ResourceUnauthorizedException(string message) : base(message)
    {
    }
}