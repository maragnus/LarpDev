namespace KiloTx.Restful;

public class BadRequestException : Exception
{
    public BadRequestException() : base("Unexpected parameters in request")
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }
}