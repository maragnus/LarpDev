namespace Larp.Data.Services;

public class UserSessionException : Exception
{
    public UserSessionException(string message) : base(message) {}
}