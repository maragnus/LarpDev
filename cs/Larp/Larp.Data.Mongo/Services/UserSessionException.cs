namespace Larp.Data.Mongo.Services;

public class UserSessionException : Exception
{
    public UserSessionException(string message) : base(message) {}
}