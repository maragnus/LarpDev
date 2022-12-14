using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Larp.Data;

public class Account
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = null!;

    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public List<AccountEmail> Emails { get; set; } = new List<AccountEmail>();
    public bool IsSuperAdmin { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? LastUpdate { get; set; }

    public Protos.Account ToProto()
    {
        var emails = Emails.Select(e => new Protos.AccountEmail()
        {
            Email = e.Email,
            IsPreferred = e.IsPreferred,
            IsVerified = e.IsVerified
        });

        var result = new Protos.Account()
        {
            AccountId = AccountId,
            Created = Created.ToString("O"),
            Location = Location ?? "",
            Name = Name ?? "",
            Phone = Phone ?? "",
            Notes = Notes ?? ""
        };
        result.Emails.AddRange(emails);

        return result;
    }
}

public class AccountEmail
{
    public string Email { get; set; } = null!;
    public string NormalizedEmail { get; set; } = null!;
    public bool IsVerified { get; set; }
    public bool IsPreferred { get; set; }
}