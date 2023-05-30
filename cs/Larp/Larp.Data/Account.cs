using System.Text.RegularExpressions;
using Larp.Common;

namespace Larp.Data;

public enum AccountRole
{
    AdminAccess,
    AccountAdmin,
    MwFifthGameMaster
}

public static class AccountRoles
{
    public const string AdminAccess = nameof(AccountRole.AdminAccess);
    public const string AccountAdmin = nameof(AccountRole.AccountAdmin);
    public const string MwFifthGameMaster = nameof(AccountRole.MwFifthGameMaster);
}

[PublicAPI]
public class AccountName
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public string? Name { get; set; }
    public List<AccountEmail> Emails { get; set; } = new();
    public AccountState State { get; set; }
}

public enum AccountState
{
    Undefined,
    Active,
    Archived,
    Uninvited
}

[PublicAPI]
public class Account
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public AccountState State { get; set; } = AccountState.Active;
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? NormalizedPhone { get; set; }
    public List<AccountEmail> Emails { get; set; } = new();
    public bool IsSuperAdmin { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? LastUpdate { get; set; }
    public DateTimeOffset? FirstLogin { get; set; }
    public DateOnly? BirthDate { get; set; }
    public int? Age => BirthDate.GetAge();
    public AccountRole[] Roles { get; set; } = Array.Empty<AccountRole>();

    public int? ImportId { get; set; }
    public int? MwFifthMoonstone { get; set; }
    public int? MwFifthUsedMoonstone { get; set; }
    public string? MwFifthPreregistrationNotes { get; set; }
    public string? AdminNotes { get; set; }

    public int? DiscountPercent { get; set; }

    public int AttachmentCount { get; set; }

    [BsonIgnore]
    public string? PreferredEmail =>
        (Emails.FirstOrDefault(x => x.IsPreferred) ?? Emails.FirstOrDefault(x => x.IsVerified))?.Email;

    [BsonIgnore] public string EmailList => string.Join(", ", Emails.Select(x => x.Email));

    public bool IsProfileComplete =>
        !string.IsNullOrWhiteSpace(Name)
        && !string.IsNullOrWhiteSpace(Location)
        && !string.IsNullOrWhiteSpace(Phone);

    public static string? BuildNormalizedPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;
        var normalizedPhone = Regex.Replace(phone.Trim(), @"[^\d]", "");

        if (normalizedPhone.Length == 10
            || (normalizedPhone.Length == 11 && normalizedPhone.StartsWith("1")))
            return normalizedPhone;

        return null;
    }
}

[PublicAPI]
public class AccountEmail
{
    public string Email { get; set; } = default!;
    public string NormalizedEmail { get; set; } = default!;
    public bool IsVerified { get; set; }
    public bool IsPreferred { get; set; }

    public static string NormalizeEmail(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        if (normalized.EndsWith("@gmail.com"))
            normalized = normalized[..normalized.IndexOf('@')].Replace(".", "") + "@gmail.com";
        return normalized;
    }
}