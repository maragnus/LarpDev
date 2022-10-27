﻿using Microsoft.Extensions.Internal;

namespace Larp.WebService.Tests;

public class TimeTravelClock : ISystemClock, Microsoft.AspNetCore.Authentication.ISystemClock
{
    public TimeTravelClock() : this(DateTimeOffset.Now) {}
    public TimeTravelClock(DateTimeOffset now) => UtcNow = now;
    
    public DateTimeOffset UtcNow { get; set; }

    public DateTimeOffset AddDays(int days)=> UtcNow += TimeSpan.FromDays(days);
    public DateTimeOffset AddHours(int days)=> UtcNow += TimeSpan.FromHours(days);
    public DateTimeOffset AddMinutes(int days)=> UtcNow += TimeSpan.FromMinutes(days);
    public DateTimeOffset AddSeconds(int days)=> UtcNow += TimeSpan.FromSeconds(days);
}