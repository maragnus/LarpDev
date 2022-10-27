namespace Larp.Data.Services;

public class UserSessionServiceOptions
{
    public const string SectionName = "UserSessionService";

    /// <summary>Number of characters to send to the user to authenticate their account</summary>
    public int TokenLength { get; set; } = 6;

    /// <summary>Frequency to check database when validating a session</summary>
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(4);

    /// <summary>Maximum duration of a new session</summary>
    public TimeSpan UserSessionDuration { get; set; } = TimeSpan.FromDays(365);
}