using Larp.Data.Services;
using Larp.Data.TestFixture;
using Larp.Test.Common;
using Larp.WebService.LarpServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace Larp.WebService.Tests;

[TestClass]
public class AuthWorkflowTests
{
    private TestDataHelper Helper { get; set; } = null!;
    private TimeTravelClock Clock { get; set; }= null!;
    private string? Token { get; set; }
    private AuthenticationService AuthService { get; set; }= null!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        Clock = new TimeTravelClock(new DateTimeOffset(2022, 6, 1, 12, 0, 0, TimeSpan.FromHours(-5)));
        Token = null;
        
        var larpDataCache = new LarpDataCache(new MemoryCacheOptions() { Clock = Clock });
        var testDataFixture = await LarpDataTestFixture.CreateTestFixtureAsync(false, Clock);
        var userSessionService = new UserSessionManager(testDataFixture.Context, larpDataCache, Clock,
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
        AuthService = new AuthenticationService(testDataFixture.Context, userSessionService, notificationService.Object, Clock);
        Helper = new TestDataHelper(context);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_Works()
    {
        await Helper.AddAccount("Josh", "acrion@gmail.com", Clock.UtcNow.AddDays(-30));

        AuthenticationResult result = null!;
        
        result = await AuthService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(Token);
        
        result = await AuthService.ConfirmLogin("acrion@gmail.com", Token);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);
        
        result = await AuthService.ValidateSession(result.SessionId);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_Expires()
    {
        await Helper.AddAccount("Josh", "acrion@gmail.com", Clock.UtcNow.AddDays(-30));

        AuthenticationResult result = null!;
        
        result = await AuthService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(Token);
        
        result = await AuthService.ConfirmLogin("acrion@gmail.com", Token);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);

        Clock.AddDays(1);

        result = await AuthService.ValidateSession(result.SessionId);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(result.SessionId);

        Clock.AddDays(1000);
        
        result = await AuthService.ValidateSession(result.SessionId);
        Assert.IsFalse(result.IsSuccess, result.Message);
        StringAssert.Contains(result.Message, "expire");
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_NotExists()
    {
        // This will create a new account
        var result = await AuthService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(Token);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_WrongToken()
    {
        await Helper.AddAccount("Josh", "acrion@gmail.com", Clock.UtcNow.AddDays(-30));

        AuthenticationResult result = null!;
        
        result = await AuthService.InitiateLogin("acrion@gmail.com", "test");
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsNotNull(Token);
        
        result = await AuthService.ConfirmLogin("acrion@gmail.com", "ABCDEF");
        Assert.IsFalse(result.IsSuccess, result.Message);
        Assert.IsNull(result.SessionId);
    }
    
    [TestMethod]
    public async Task AuthenticationWorkflow_NoInitiate()
    {
        await Helper.AddAccount("Josh", "acrion@gmail.com", Clock.UtcNow.AddDays(-30));

        // This code isn't possible to generate but meets entry criteria
        const string code = "AB0O1L"; 
        
        // Attempt to authenticate with the wrong code
        var result = await AuthService.ConfirmLogin("acrion@gmail.com", code);
        Assert.IsFalse(result.IsSuccess, result.Message);
        Assert.IsNull(result.SessionId);
    }
}