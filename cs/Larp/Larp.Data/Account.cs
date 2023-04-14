using System.Text.Json.Serialization;
using Larp.Common;

namespace Larp.Data;

public enum AccountRole
{
    AdminAccess,
    AccountAdmin,
    MwFifthGameMaster
}

[PublicAPI]
public class AccountName
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public string? Name { get; set; }
    public List<AccountEmail> Emails { get; set; } = new();
}

[PublicAPI]
public class Account
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
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

    public int? DiscountPercent { get; set; }
    
    [BsonIgnore]
    public string? PreferredEmail =>
        (Emails.FirstOrDefault(x => x.IsPreferred) ?? Emails.FirstOrDefault(x => x.IsVerified))?.Email;

    [BsonIgnore] public string? EmailList => string.Join(", ", Emails.Select(x => x.Email));

    public bool IsProfileComplete =>
        !string.IsNullOrWhiteSpace(Name)
        && !string.IsNullOrWhiteSpace(Location)
        && !string.IsNullOrWhiteSpace(Phone);
}

[PublicAPI]
public class AccountEmail
{
    public string Email { get; set; } = default!;
    public string NormalizedEmail { get; set; } = default!;
    public bool IsVerified { get; set; }
    public bool IsPreferred { get; set; }
}

public class AccountAttachment
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string AttachmentId { get; set; } = default!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string UploadedBy { get; set; } = default!;

    public DateTimeOffset UploadedOn { get; set; }
    
    public string Title { get; set; } = default!;
    
    public string? MediaType { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public byte[]? Data { get; set; }
    public string? FileName { get; set; }
}