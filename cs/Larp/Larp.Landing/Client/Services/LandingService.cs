using Blazored.LocalStorage;
using Larp.Data;
using Larp.Landing.Shared;
using Larp.Landing.Shared.MwFifth;
using PSC.Blazor.Components.BrowserDetect;

namespace Larp.Landing.Client.Services;

public enum AutoSaveState
{
    Inactive,
    ChangeAvailable,
    Saved,
    Saving
}

public class LandingService
{
    private readonly ILandingService _landing;
    private readonly IAdminService _adminService;
    private readonly DataCacheService _dataCache;
    private readonly ILogger<LandingService> _logger;
    private readonly ILocalStorageService _localStorage;
    private readonly LandingServiceClient _client;

    private const int GameStateUpdateFrequencyHours = 4;
    private const string SessionIdKey = "SessionId";

    public LandingService(ILandingService landing,
        IAdminService adminService,
        IMwFifthService mwFifth,
        DataCacheService dataCache,
        ILogger<LandingService> logger, 
        ILocalStorageService localStorage,
        LandingServiceClient client)
    {
        _landing = landing;
        _adminService = adminService;
        _dataCache = dataCache;
        _logger = logger;
        _localStorage = localStorage;
        _localStorage.Changed += LocalStorageOnChanged;
        _client = client;
        MwFifth = new MwFifthService(this, mwFifth, localStorage, dataCache);
        Admin = adminService;
    }

    public IReadOnlyDictionary<string, Game> Games { get; private set; } = null!;
    public MwFifthService MwFifth { get; }
    public string? SessionId { get; private set; }
    public bool IsAuthenticated { get; private set; }
    public event EventHandler? AuthenticatedChanged;
    public async Task<CharacterSummary[]> GetCharacters() => await _landing.GetCharacters();
    public BrowserInfo? BrowserInfo { get; set; }
    public string? LocationName { get; set; }
    public Account? Account { get; set; }
    public IAdminService Admin { get; }
    
    private void LocalStorageOnChanged(object? sender, ChangedEventArgs e)
    {
        if (e.Key == SessionIdKey)
            SetSessionId((string)e.NewValue);
    }
    
    private void SetSessionId(string? sessionId)
    {
        _client.SetSessionId(sessionId);
        SessionId = sessionId;
        IsAuthenticated = !string.IsNullOrEmpty(sessionId);
        AuthenticatedChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public async Task Refresh()
    {
        var sessionId = await _localStorage.GetItemAsStringAsync(SessionIdKey);
        SetSessionId(sessionId);
        _logger.LogInformation("Refresh starting...");
        Account = await GetAccount();
        await GetGames();
        await MwFifth.Refresh();
        _logger.LogInformation("Refresh complete");
    }

    private async Task GetGames()
    {
        Games = await _dataCache
            .CacheItem(nameof(Games), TimeSpan.FromHours(GameStateUpdateFrequencyHours), async () =>
            {
                var games = await _landing.GetGames();
                return games.ToDictionary(x => x.Name);
            });
    }

    public async Task Login(string email)
    {
        await _landing.Login(email, LocationName ?? "unknown");
    }
    
    public async Task LoginConfirm(string email, string token, string deviceName)
    {
        var sessionId = await _landing.Confirm(email, token, deviceName);
        if (!sessionId.IsSuccess)
            throw new Exception(sessionId.ErrorMessage);
        await _localStorage.SetItemAsStringAsync(SessionIdKey, sessionId.Value ?? "");
        SetSessionId(sessionId.Value);
    }
    
    public async Task<bool> Logout(bool force)
    {
        try
        {
            await _landing.Logout();
            await _localStorage.RemoveItemAsync(SessionIdKey);
            SetSessionId(null);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log out");
            if (!force)
                throw;
            
            await _localStorage.RemoveItemAsync(SessionIdKey);
            SetSessionId(null);
            return false;
        }
    }

    public void UpdateBrowserInfo(BrowserInfo info)
    {
        BrowserInfo = info;
        LocationName =
            $"{info.BrowserName} on {info.OSName} {info.OSVersion} {info.DeviceModel} {info.DeviceType}".Replace("  ", " ").Trim();
    }

    public async Task ValidateSession()
    {
        if (!IsAuthenticated)
            return;
        
        try
        {
            var result = await _landing.Validate();
            if (!result.IsSuccess)
                SetSessionId(null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate session");
        }
    }

    public async Task Reset()
    {
        await Logout(true);
        await _localStorage.ClearAsync();
    }

    public async Task<Account> GetAccount()
    {
        Account = await _landing.GetAccount();
        AuthenticatedChanged?.Invoke(this, EventArgs.Empty);
        return Account;
    }

    public async Task AccountEmailAdd(string email) =>
        await _landing.AccountEmailAdd(email);
    
    public async Task AccountEmailRemove(string email) =>
        await _landing.AccountEmailRemove(email);
    
    public async Task AccountEmailPreferred(string email) =>
        await _landing.AccountEmailPreferred(email);

    public async Task AccountUpdate(string? fullName, string? location, string? phone, string? allergies,
        DateOnly? birthDate) =>
        await _landing.AccountUpdate(fullName, location, phone, allergies, birthDate);
}