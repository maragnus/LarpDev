using System.Net;
using Blazored.LocalStorage;
using KiloTx.Restful;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Client.RestClient;
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
    public const string ServiceName = "Mystwood Tavern";

    private readonly DataCacheService _dataCache;
    private readonly ILogger<LandingService> _logger;
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClientFactory _httpClientFactory;

    private const int GameStateUpdateFrequencyHours = 4;
    private const string SessionIdKey = "SessionId";

    public LandingService(ILandingService landing,
        IAdminService admin,
        IMwFifthService mwFifth,
        DataCacheService dataCache,
        ILogger<LandingService> logger,
        ILocalStorageService localStorage,
        HttpClientFactory httpClientFactory)
    {
        Service = landing;
        Admin = admin;
        MwFifth = mwFifth;
        _dataCache = dataCache;
        _logger = logger;
        _localStorage = localStorage;
        _localStorage.Changed += LocalStorageOnChanged;
        _httpClientFactory = httpClientFactory;
    }

    public IReadOnlyDictionary<string, Game> Games { get; private set; } = default!;
    public IAdminService Admin { get; }
    public ILandingService Service { get; }
    public IMwFifthService MwFifth { get; }
    public bool IsAuthenticated { get; private set; }
    public event EventHandler? AuthenticatedChanged;
    public async Task<CharacterSummary[]> GetCharacters() => await Service.GetCharacters();
    public BrowserInfo? BrowserInfo { get; private set; }
    public string? LocationName { get; private set; }
    public Account? Account { get; private set; }

    public Game MwFifthGame => Games[GameState.GameName];
    public GameState MwFifthGameState { get; private set; } = default!;
    
    private async Task GetMwFifthGameState()
    {
        MwFifthGameState = await _dataCache.CacheGameState<GameState>(GameState.GameName,
            async revision => await MwFifth.GetGameState(revision));
    }

    private void LocalStorageOnChanged(object? sender, ChangedEventArgs e)
    {
        if (e.Key == SessionIdKey)
            SetSessionId((string)e.NewValue);
    }

    private void SetSessionId(string? sessionId)
    {
        _httpClientFactory.SetAuthenticationToken(sessionId);
        IsAuthenticated = !string.IsNullOrEmpty(sessionId);
        InvokeAuthenticatedChanged();
    }

    public async Task Refresh()
    {
        _logger.LogInformation("Refresh");
        var sessionId = await _localStorage.GetItemAsStringAsync(SessionIdKey);
        SetSessionId(sessionId);
        _logger.LogInformation("Refresh starting...");
        await Task.WhenAll(
            GetGames(),
            GetMwFifthGameState(),
            GetAccount());
        _logger.LogInformation("Refresh complete");
    }

    private void InvokeAuthenticatedChanged()
    {
        AuthenticatedChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task GetGames()
    {
        Games = await _dataCache
            .CacheItem(nameof(Games), TimeSpan.FromHours(GameStateUpdateFrequencyHours), async () =>
            {
                var games = await Service.GetGames();
                return games.ToDictionary(x => x.Name);
            });
    }

    public async Task Login(string email)
    {
        await Service.Login(email, LocationName ?? "unknown");
    }

    public async Task LoginConfirm(string email, string token, string deviceName)
    {
        var sessionId = await Service.Confirm(email, token, deviceName);
        if (!sessionId.IsSuccess)
            throw new Exception(sessionId.ErrorMessage);
        await _localStorage.SetItemAsStringAsync(SessionIdKey, sessionId.Value ?? "");
        SetSessionId(sessionId.Value);
    }

    public async Task Logout(bool force)
    {
        try
        {
            await Service.Logout();
            await _localStorage.RemoveItemAsync(SessionIdKey);
            SetSessionId(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log out");
            if (!force)
                throw;

            await _localStorage.RemoveItemAsync(SessionIdKey);
            SetSessionId(null);
        }
    }

    public void UpdateBrowserInfo(BrowserInfo info)
    {
        BrowserInfo = info;
        LocationName =
            $"{info.BrowserName} on {info.OSName} {info.OSVersion} {info.DeviceModel} {info.DeviceType}"
                .Replace("  ", " ").Trim();
    }
    
    public async Task Reset()
    {
        await Logout(true);
        await _localStorage.ClearAsync();
    }

    public async Task<Account> GetAccount()
    {
        _logger.LogInformation("GetAccount");
        try
        {
            Account = await Service.GetAccount();
            InvokeAuthenticatedChanged();
            return Account;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            Account = null;
            SetSessionId(null);
            return new Account();
        }
        catch (BadRequestException ex)
        {
            _logger.LogError(ex, "GetAccount failed");
            Account = null;
            SetSessionId(null);
            return new Account();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAccount failed");
            throw;
        }
    }

    public bool IsInRole(AccountRole role) =>
        Account?.Roles?.Contains(role) == true;
}