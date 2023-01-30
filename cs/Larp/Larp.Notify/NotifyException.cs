namespace Larp.Notify;

public class NotifyException : Exception
{
    public NotifyException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}