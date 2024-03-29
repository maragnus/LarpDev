﻿namespace Larp.Data.Mongo.Services;

public class UserSessionManagerOptions
{
    public const string SectionName = "UserSessionManager";

    /// <summary>Number of characters to send to the user to authenticate their account</summary>
    public int TokenLength { get; set; } = 4;

    /// <summary>Frequency to check database when validating a session</summary>
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>Maximum duration of a new session</summary>
    public TimeSpan UserSessionDuration { get; set; } = TimeSpan.FromDays(365);
}