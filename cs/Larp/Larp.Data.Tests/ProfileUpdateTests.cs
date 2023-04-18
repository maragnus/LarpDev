using Larp.Data.Mongo.Services;
using Larp.Data.TestFixture;
using Larp.Test.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Larp.Data.Tests;

[TestClass]
public class ProfileUpdateTests
{
    private TestDataHelper Helper { get; set; } = null!;
    private TimeTravelClock Clock { get; set; } = null!;
    private string? Token { get; set; }
    private UserSessionManager UserSessionManager { get; set; } = null!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        Clock = new TimeTravelClock(new DateTimeOffset(2022, 6, 1, 12, 0, 0, TimeSpan.FromHours(-5)));
        Token = null;

        var larpDataCache = new LarpDataCache(new MemoryCacheOptions() { Clock = Clock });
        var testDataFixture = await LarpDataTestFixture.CreateTestFixtureAsync(false, Clock);
        UserSessionManager = new UserSessionManager(testDataFixture.Context, larpDataCache, Clock,
            Options.Create(new UserSessionManagerOptions
            {
                CacheDuration = TimeSpan.FromMinutes(10),
                UserSessionDuration = TimeSpan.FromDays(5)
            }), new NullLogger<UserSessionManager>());
        var context = testDataFixture.Context;
        Helper = new TestDataHelper(context);
    }

    [TestMethod]
    public async Task AddEmailOnce_Works()
    {
        var accountId = await Helper.AddAccount("Bob Dylan", "test+bob@maragnus.com", Clock.UtcNow);
        await UserSessionManager.AddEmailAddress(accountId, "test+rob@maragnus.com");
        var account = await UserSessionManager.GetUserAccount(accountId);
        Assert.AreEqual(2, account.Emails.Count);
        Assert.IsTrue(account.Emails.Any(x => x.Email == "test+bob@maragnus.com"));
        Assert.IsTrue(account.Emails.Any(x => x.Email == "test+rob@maragnus.com"));

        // Add duplicates
        await UserSessionManager.AddEmailAddress(accountId, "test+bob@maragnus.com");
        await UserSessionManager.AddEmailAddress(accountId, "test+rob@maragnus.com");
        account = await UserSessionManager.GetUserAccount(accountId);
        Assert.AreEqual(2, account.Emails.Count);
    }

    [TestMethod]
    public async Task AddEmailDuplicateDoesntResetFlags_Works()
    {
        var accountId = await Helper.AddAccount("Bob Dylan", "test+bob@maragnus.com", Clock.UtcNow);
        var account = await UserSessionManager.GetUserAccount(accountId);
        Assert.IsFalse(account.Emails[0].IsPreferred);

        // Flip Preferred to True
        await UserSessionManager.PreferEmailAddress(accountId, "test+bob@maragnus.com");
        account = await UserSessionManager.GetUserAccount(accountId);
        Assert.IsTrue(account.Emails[0].IsPreferred);

        // Add duplicate email, IsPreferred stays true
        await UserSessionManager.AddEmailAddress(accountId, "test+bob@maragnus.com");
        account = await UserSessionManager.GetUserAccount(accountId);
        Assert.IsTrue(account.Emails[0].IsPreferred);
        Assert.AreEqual(1, account.Emails.Count);
    }

    [TestMethod]
    public async Task AddEmailPreventDuplicatesAcrossAccounts_Works()
    {
        var accountId1 = await Helper.AddAccount("Bob Dylan", "test+bob@maragnus.com", Clock.UtcNow);
        var accountId2 = await Helper.AddAccount("Robert Zimmerman", "test+rob@maragnus.com", Clock.UtcNow);

        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await UserSessionManager.AddEmailAddress(accountId2, "test+bob@maragnus.com");
        });

        var account1 = await UserSessionManager.GetUserAccount(accountId1);
        Assert.IsTrue(account1.Emails.Any(x => x.Email == "test+bob@maragnus.com"));

        var account2 = await UserSessionManager.GetUserAccount(accountId2);
        Assert.IsTrue(account2.Emails.Any(x => x.Email == "test+rob@maragnus.com"));
    }

    [TestMethod]
    public async Task EmailCanChangeAccounts_Works()
    {
        var accountId1 = await Helper.AddAccount("Bob Dylan", "test+bob@maragnus.com", Clock.UtcNow);
        var accountId2 = await Helper.AddAccount("Robert Zimmerman", "test+rob@maragnus.com", Clock.UtcNow);

        await UserSessionManager.RemoveEmailAddress(accountId1, "test+bob@maragnus.com");
        await UserSessionManager.AddEmailAddress(accountId2, "test+bob@maragnus.com");

        var account1 = await UserSessionManager.GetUserAccount(accountId1);
        var account2 = await UserSessionManager.GetUserAccount(accountId2);

        Assert.IsFalse(account1.Emails.Any(x => x.Email == "test+bob@maragnus.com"));
        Assert.IsTrue(account2.Emails.Any(x => x.Email == "test+bob@maragnus.com"));
    }
}