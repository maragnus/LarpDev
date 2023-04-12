namespace Larp.Common.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base("Unexpected parameters in request")
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }
}