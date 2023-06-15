namespace Larp.Data;

[PublicAPI]
public class ClarifyTerm
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string GameId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Summary { get; set; }

    public string? Description { get; set; }
}