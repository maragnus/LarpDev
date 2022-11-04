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
}

public class AccountEmail
{
    public string Email { get; set; } = null!;
    public bool IsVerified { get; set; }
    public bool IsPreferred { get; set; }
}