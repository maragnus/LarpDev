using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using Larp.Landing.Shared.MwFifth;
using Microsoft.Extensions.FileProviders;
using MwFifthCharacter = Larp.Data.MwFifth.Character;

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

    public Task AccountUpdate(string? fullName, string? location, string? phone, string? allergies,
        DateOnly? birthDate) =>
        Post("api/account", new { fullName, location, phone, allergies, birthDate });

    public Task<Event[]> GetEvents() =>
        Get<Event[]>("api/events");

    Task<Event> IAdminService.GetEvent(string eventId) =>
        Get<Event>($"api/admin/events/{eventId}");

    public Task<GameState> GetGameState(string lastRevision) =>
        Get<GameState>($"api/mw5e/gameState?lastRevision={lastRevision}");

    public Task<Character> GetCharacter(string characterId) =>
        Get<Character>($"api/mw5e/character?characterId={characterId}");

    public Task<Character> ReviseCharacter(string characterId) =>
        Get<Character>($"api/mw5e/character/revise?characterId={characterId}");

    public Task<Character> GetNewCharacter() =>
        Get<Character>($"api/mw5e/character/new")!;

    public Task SaveCharacter(Character character) =>
        Post("api/mw5e/character", new { character });

    public async Task DeleteCharacter(string characterId) =>
        await Delete($"api/mw5e/character?characterId={characterId}");

    public Task<Account[]> GetAccounts() =>
        Get<Account[]>($"api/admin/accounts");

    public Task<StringResult> AddAdminAccount(string fullName, string emailAddress) =>
        Post<StringResult>($"api/admin/accounts/admin", new {fullName, emailAddress});
    
    public Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state) =>
        Get<CharacterAccountSummary[]>($"api/admin/mw5e/characters?state={state}");

    public Task<MwFifthCharacter> GetMwFifthCharacter(string characterId) =>
        Get<MwFifthCharacter>($"api/admin/mw5e/characters/{characterId}");

    public Task<Account> GetAccount(string accountId) =>
        Get<Account>($"api/admin/accounts/{accountId}");

    public Task<CharacterSummary[]> GetAccountCharacters(string accountId) =>
        Get<CharacterSummary[]>($"api/admin/accounts/{accountId}/characters");

    public Task UpdateAccount(string accountId, string? name, string? location,
        string? phone, DateOnly? birthDate, string? notes) =>
        Post($"api/admin/accounts/{accountId}", new { name, location, phone, birthDate, notes });

    public Task AddAccountRole(string accountId, AccountRole role) =>
        Post($"api/admin/accounts/{accountId}/roles/{role}");

    public Task RemoveAccountRole(string accountId, AccountRole role) =>
        Delete($"api/admin/accounts/{accountId}/roles/{role}");

    public Task<MwFifthCharacter> GetMwFifthCharacterLatest(string characterId) =>
        Get<MwFifthCharacter>($"api/admin/mw5e/characters/{characterId}/latest");

    public Task<MwFifthCharacter[]> GetMwFifthCharacterRevisions(string characterId) =>
        Get<MwFifthCharacter[]>($"api/admin/mw5e/characters/{characterId}/revisions");

    public Task ApproveMwFifthCharacter(string characterId) =>
        Post($"api/admin/mw5e/characters/{characterId}/approve");

    public Task RejectMwFifthCharacter(string characterId) =>
        Post($"api/admin/mw5e/characters/{characterId}/reject");

    public Task<Dashboard> GetDashboard() =>
        Get<Dashboard>($"api/admin/dashboard");

    public Task<Character> ReviseMwFifthCharacter(string characterId) =>
        Post<Character>($"api/admin/mw5e/characters/{characterId}/revise");

    public Task SaveMwFifthCharacter(Character character) =>
        Post($"api/admin/mw5e/characters", new { character });

    public Task DeleteMwFifthCharacter(string characterId) =>
        Delete($"api/admin/mw5e/characters/{characterId}");

    Task<Event[]> IAdminService.GetEvents() =>
        Get<Event[]>($"api/admin/events");
}