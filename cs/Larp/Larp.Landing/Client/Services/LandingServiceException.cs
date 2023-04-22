namespace Larp.Landing.Client.Services;

public class LandingServiceException : Exception
{
    public LandingServiceException(string message) : base(message) { }
    public LandingServiceException(string message, Exception exception) : base(message, exception) { }
}