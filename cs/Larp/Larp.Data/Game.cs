namespace Larp.Data;

[PublicAPI]
public class Game
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Title { get; set; }

    public string? Description { get; set; }
    
    public string? Email { get; set; }
}