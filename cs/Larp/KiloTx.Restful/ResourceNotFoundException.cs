namespace KiloTx.Restful;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException() : base("Resource was not found")
    {
    }

    public ResourceNotFoundException(string message) : base(message)
    {
    }
}
