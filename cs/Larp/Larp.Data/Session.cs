namespace Larp.Data;

[PublicAPI]
public class Session
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string SessionId { get; set; } = null!;

    public string? Token { get; set; }
    public string? Email { get; set; }
    public string AccountId { get; set; } = null!;
    public string? DeviceId { get; set; }
    public bool IsAuthenticated { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
    public DateTimeOffset? DestroyedOn { get; set; }
    public DateTimeOffset? ActivatedOn { get; set; }
}