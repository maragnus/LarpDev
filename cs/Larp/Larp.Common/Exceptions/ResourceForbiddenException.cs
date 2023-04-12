namespace Larp.Common.Exceptions;

public class ResourceForbiddenException : Exception
{
    public ResourceForbiddenException() : base("User is authenticated but not privileged to access this resource")
    {
    }

    public ResourceForbiddenException(string message) : base(message)
    {
    }
}