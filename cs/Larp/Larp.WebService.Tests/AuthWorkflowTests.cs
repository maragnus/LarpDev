using Larp.Data.Services;
using Larp.Data.TextFixture;
using Larp.WebService.LarpServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace Larp.WebService.Tests;

[TestClass]
public class AuthWorkflowTests
{
    private DataHelper helper;
    private TimeTravelClock clock;
    private string? token;
    private AuthenticationService authService;

    [TestInitialize]
    public async Task TestInitialize()
    {
        clock = new TimeTravelClock(new DateTimeOffset(2022, 6, 1, 12, 0, 0, TimeSpan.FromHours(-5)));
        token = null;
        
        var larpDataCache = new LarpDataCache(new MemoryCacheOptions() { Clock = clock });
        var testDataFixture = await LarpDataTestFixture.CreateTestFixtureAsync(false);
        var userSessionService = new UserSessionManager(testDataFixture.Context, larpDataCache, clock,
            Options.Create<UserSessionManagerOptions>(new()
            {
                CacheDuration = TimeSpan.FromMinutes(10),
                UserSessionDuration = TimeSpan.FromDays(5)
            }));
        var context = testDataFixture.Context;
        var notificationService = new Mock<IUserNotificationService>();
        notificationService
            .Setup(x => x.SendAuthenticationToken(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((_, tokenCode) => token = tokenCode);
        authService = new AuthenticationService(testDataFixture.Context, userSessionService, notificationService.Object, clock);
        helper = new DataHelper(context);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_Works()
    {
        await helper.AddAccount("Josh", "acrion@gmail.com", clock.UtcNow.AddDays(-30));

        AuthenticationResult result = null!;
        
        result = await authService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(token);
        
        result = await authService.ConfirmLogin("acrion@gmail.com", token);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);
        
        result = await authService.ValidateSession(result.SessionId);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_Expires()
    {
        await helper.AddAccount("Josh", "acrion@gmail.com", clock.UtcNow.AddDays(-30));

        AuthenticationResult result = null!;
        
        result = await authService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(token);
        
        result = await authService.ConfirmLogin("acrion@gmail.com", token);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);

        clock.AddDays(1);

        result = await authService.ValidateSession(result.SessionId);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);

        clock.AddDays(1000);
        
        result = await authService.ValidateSession(result.SessionId);
        Assert.IsFalse(result.IsSuccess, result.Message);
        StringAssert.Contains(result.Message, "expire");
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_NotExists()
    {
        AuthenticationResult result = null!;
        
        result = await authService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsFalse(result.IsSuccess, result.Message);
        Assert.IsNull(token);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_WrongToken()
    {
        await helper.AddAccount("Josh", "acrion@gmail.com", clock.UtcNow.AddDays(-30));

        AuthenticationResult result = null!;
        
        result = await authService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(token);
        
        result = await authService.ConfirmLogin("acrion@gmail.com", "ABCDEF");
        Assert.IsFalse(result.IsSuccess, result.Message);
        Assert.IsNull(result.SessionId);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_NoInitiate()
    {
        await helper.AddAccount("Josh", "acrion@gmail.com", clock.UtcNow.AddDays(-30));

        AuthenticationResult result = null!;
        
        result = await authService.ConfirmLogin("acrion@gmail.com", "ABCDEF");
        Assert.IsFalse(result.IsSuccess, result.Message);
        Assert.IsNull(result.SessionId);
    }
}