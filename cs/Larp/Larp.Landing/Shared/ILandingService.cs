using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api")]
public interface ILandingService
{
    [ApiPost("auth/login")]
    Task<Result> Login(string email, string deviceName);

    [ApiPost("auth/confirm")]
    Task<StringResult> Confirm(string email, string token, string deviceName);

    [ApiPost("auth/logout")]
    Task<Result> Logout();

    [ApiPost("auth/validate"), ApiAuthenticated]
    Task<Result> Validate();

    [ApiGet("larp/games")]
    Task<Game[]> GetGames();

    [ApiGet("larp/characters"), ApiAuthenticated]
    Task<CharacterSummary[]> GetCharacters();

    [ApiGet("account"), ApiAuthenticated]
    Task<Account> GetAccount();

    [ApiPost("account/email"), ApiAuthenticated]
    Task AccountEmailAdd(string email);

    [ApiDelete("account/email"), ApiAuthenticated]
    Task AccountEmailRemove(string email);

    [ApiPost("account/email/preferred"), ApiAuthenticated]
    Task AccountEmailPreferred(string email);

    [ApiPost("account"), ApiAuthenticated]
    Task AccountUpdate(string? fullName, string? location, string? phone, string? allergies, DateOnly? birthDate);

    [ApiGet("dashboard")]
    Task<AccountDashboard> GetDashboard();

    [ApiGet("events")]
    Task<EventAttendanceList> GetEvents(EventList list);

    [ApiGet("larp/characters/names"), ApiAuthenticated]
    Task<Dictionary<string, string>> GetCharacterNames();

    [ApiGet("events/attendance"), ApiAuthenticated]
    Task<EventAttendanceList> GetAttendance();

    [ApiPost("letters/new"), ApiAuthenticated]
    Task<Letter> DraftLetter(string eventId, string letterName);

    [ApiGet("letters/{letterId}"), ApiAuthenticated]
    Task<Letter> GetLetter(string letterId);

    [ApiPost("letters/{letterId}"), ApiAuthenticated]
    Task SaveLetter(string letterId, Letter letter);

    [ApiGet("letters/events/{eventId}/{letterName}"), ApiAuthenticated]
    Task<EventsAndLetters> GetEventLetter(string eventId, string letterName);

    [ApiGet("attachments/{attachmentId}/{fileName}")]
    Task<IFileInfo> GetAttachment(string attachmentId, string fileName);

    [ApiGet("attachments/thumbnails/{attachmentId}/{fileName}")]
    Task<IFileInfo> GetAttachmentThumbnail(string attachmentId, string fileName);

    [ApiPost("letters/templates/{letterTemplateId}"), ApiAuthenticated]
    Task<LetterTemplate> GetLetterTemplate(string letterTemplateId);

    [ApiPost("transactions"), ApiAuthenticated]
    Task<Transaction[]> GetTransactions();
}