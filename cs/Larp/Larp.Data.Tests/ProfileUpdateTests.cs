using Larp.Data.Mongo.Services;
using Larp.Data.TestFixture;
using Larp.Test.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace Larp.Data.Tests;

[TestClass]
public class ProfileUpdateTests
{
    private TestDataHelper Helper { get; set; } = null!;
    private TimeTravelClock Clock { get; set; } = null!;
    private string? Token { get; set; }
    private AuthenticationService AuthService { get; set; } = null!;
    private UserSessionManager UserSessionManager { get; set; } = null!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        Clock = new TimeTravelClock(new DateTimeOffset(2022, 6, 1, 12, 0, 0, TimeSpan.FromHours(-5)));
        Token = null;

        var larpDataCache = new LarpDataCache(new MemoryCacheOptions() { Clock = Clock });
        var testDataFixture = await LarpDataTestFixture.CreateTestFixtureAsync(false, Clock);
        UserSessionManager = new UserSessionManager(testDataFixture.Context, larpDataCache, Clock,
            Options.Create<UserSessionManagerOptions>(new()
            {
                CacheDuration = TimeSpan.FromMinutes(10),
                UserSessionDuration = TimeSpan.FromDays(5)
            }));
        var context = testDataFixture.Context;
        var notificationService = new Mock<IUserNotificationService>();
        notificationService
            .Setup(x => x.SendAuthenticationToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, _, tokenCode) => Token = tokenCode);
        AuthService = new AuthenticationService(testDataFixture.Context, UserSessionManager, notificationService.Object,
            Clock);
        Helper = new TestDataHelper(context);
    }

    [TestMethod]
    public async Task AddEmailOnce_Works()
    {
        var accountId = await Helper.AddAccount("Bob Dylan", "acrion@gmail.com", Clock.UtcNow);
        await UserSessionManager.AddEmailAddress(accountId, "bob@dylan.com");
        var account = await UserSessionManager.GetUserAccount(accountId);
        Assert.AreEqual(2, account.Emails.Count);
        Assert.IsTrue(account.Emails.Any(x => x.Email == "acrion@gmail.com"));
        Assert.IsTrue(account.Emails.Any(x => x.Email == "bob@dylan.com"));

        // Add duplicates
        await UserSessionManager.AddEmailAddress(accountId, "acrion@gmail.com");
        await UserSessionManager.AddEmailAddress(accountId, "bob@dylan.com");
        account = await UserSessionManager.GetUserAccount(accountId);
        Assert.AreEqual(2, account.Emails.Count);
    }

    [TestMethod]
    public async Task AddEmailDuplicateDoesntResetFlags_Works()
    {
        var accountId = await Helper.AddAccount("Bob Dylan", "acrion@gmail.com", Clock.UtcNow);
        var account = await UserSessionManager.GetUserAccount(accountId);
        Assert.IsFalse(account.Emails[0].IsPreferred);

        // Flip Preferred to True
        await UserSessionManager.PreferEmailAddress(accountId, "acrion@gmail.com");
        account = await UserSessionManager.GetUserAccount(accountId);
        Assert.IsTrue(account.Emails[0].IsPreferred);

        // Add duplicate email, IsPreferred stays true
        await UserSessionManager.AddEmailAddress(accountId, "acrion@gmail.com");
        account = await UserSessionManager.GetUserAccount(accountId);
        Assert.IsTrue(account.Emails[0].IsPreferred);
        Assert.AreEqual(1, account.Emails.Count);
    }


    [TestMethod]
    public async Task AddEmailPreventDuplicatesAcrossAccounts_Works()
    {
        var accountId1 = await Helper.AddAccount("Bob Dylan", "bob@example.com", Clock.UtcNow);
        var accountId2 = await Helper.AddAccount("Robert Zimmerman", "robert@example.com", Clock.UtcNow);

        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await UserSessionManager.AddEmailAddress(accountId2, "bob@example.com");
        });


        var account1 = await UserSessionManager.GetUserAccount(accountId1);
        Assert.IsTrue(account1.Emails.Any(x => x.Email == "bob@example.com"));

        var account2 = await UserSessionManager.GetUserAccount(accountId2);
        Assert.IsTrue(account2.Emails.Any(x => x.Email == "robert@example.com"));
    }

    [TestMethod]
    public async Task EmailCanChangeAccounts_Works()
    {
        var accountId1 = await Helper.AddAccount("Bob Dylan", "bob@example.com", Clock.UtcNow);
        var accountId2 = await Helper.AddAccount("Robert Zimmerman", "robert@example.com", Clock.UtcNow);

        await UserSessionManager.RemoveEmailAddress(accountId1, "bob@example.com");
        await UserSessionManager.AddEmailAddress(accountId2, "bob@example.com");

        var account1 = await UserSessionManager.GetUserAccount(accountId1);
        var account2 = await UserSessionManager.GetUserAccount(accountId2);

        Assert.IsFalse(account1.Emails.Any(x => x.Email == "bob@example.com"));
        Assert.IsTrue(account2.Emails.Any(x => x.Email == "bob@example.com"));
    }
}