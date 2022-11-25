using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Larp.Data;

public class Game
{
    public Game()
    {
    }

    public Game(Protos.Game game)
    {
        Name = game.Name;
        Title = game.Title;
        Description = game.Description;
    }

    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public Protos.Game ToProto()
    {
        return new Protos.Game()
        {
            Description = Description,
            Name = Name,
            Title = Title
        };
    }
}