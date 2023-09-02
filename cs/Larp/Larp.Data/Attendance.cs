namespace Larp.Data;

[PublicAPI]
public class Attendance
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    [BsonIgnoreIfNull] public decimal? ProvidedPayment { get; set; }

    [BsonIgnoreIfNull] public decimal? ExpectedPayment { get; set; }

    public MwFifthAttendance? MwFifth { get; set; }
}