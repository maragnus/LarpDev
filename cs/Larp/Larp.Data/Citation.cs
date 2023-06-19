namespace Larp.Data;

public enum CitationState
{
    Draft,
    Open,
    Resolved
}

public class Citation
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AuthorAccountId { get; set; } = default!;

    public string? Type { get; set; }

    public CitationState State { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }
}