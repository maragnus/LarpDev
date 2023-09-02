namespace Larp.Data;

public class MwFifthAttendance
{
    public int? Moonstone { get; set; }

    public int? PostMoonstone { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string[] CharacterIds { get; set; } = Array.Empty<string>();
}