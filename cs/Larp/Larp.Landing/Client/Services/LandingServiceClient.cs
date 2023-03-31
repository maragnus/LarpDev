using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using Larp.Landing.Shared.MwFifth;
using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Client.Services;

public class LandingServiceClient : RestClient, ILandingService, IMwFifthService, IAdminService
{
    public LandingServiceClient(HttpClient httpClient, ILogger<LandingServiceClient> logger) 
        : base(httpClient, logger)
    {
    }

    public async Task<Result> Login(string email, string deviceName) =>
        await Post<Result>("api/auth/login", new { Email = email, DeviceName = deviceName });

    public async Task<StringResult> Confirm(string email, string token, string deviceName) =>
        await Post<StringResult>("api/auth/confirm",
            new { Email = email, Token = token, DeviceName = deviceName });

    public async Task<Result> Logout() =>
        await Post<Result>("api/auth/logout");

    public async Task<Result> Validate() =>
        await Post<Result>("api/auth/validate");

    public async Task<Game[]> GetGames() =>
        await GetArray<Game>("api/larp/games");

    public async Task<CharacterSummary[]> GetCharacters() =>
        await GetArray<CharacterSummary>("api/larp/characters");

    public Task<IFileInfo> Export()
    {
        // This is server-side only
        return Task.FromResult((IFileInfo)null!);
    }

    public Task<Account> GetAccount() =>
        Get<Account>("api/account");

    public Task AccountEmailAdd(string email) =>
        Post($"api/account/email?email={Uri.EscapeDataString(email)}");

    public Task AccountEmailRemove(string email) =>
        Delete($"api/account/email?email={Uri.EscapeDataString(email)}");

    public Task AccountEmailPreferred(string email) =>
        Post($"api/account/email/preferred?email={Uri.EscapeDataString(email)}");

    public Task AccountUpdate(string? fullName, string? location, string? phone, string? allergies, DateOnly? birthDate) =>
        Post("api/account", new { fullName, location, phone, allergies, birthDate });

    public Task<GameState> GetGameState(string lastRevision) =>
        Get<GameState>($"api/mw5e/gameState?lastRevision={lastRevision}");

    public Task<Character> GetCharacter(string characterId) =>
        Get<Character>($"api/mw5e/character?characterId={characterId}");

    public Task<Character> ReviseCharacter(string characterId) =>
        Get<Character>($"api/mw5e/character/revise?characterId={characterId}");

    public Task<Character> GetNewCharacter() =>
        Get<Character>($"api/mw5e/character/new")!;

    public Task<StringResult> SaveCharacter(Character character) =>
        Post<StringResult>("api/mw5e/character", new { character });

    public async Task DeleteCharacter(string characterId) =>
        await Delete($"api/mw5e/character?characterId={characterId}");

    public Task<Account[]> GetAccounts() =>
        Get<Account[]>($"api/admin/accounts");
    
    public Task<CharacterAccountSummary[]> GetMwFifthCharacters() =>
        Get<CharacterAccountSummary[]>("api/admin/mw5e/characters");
}