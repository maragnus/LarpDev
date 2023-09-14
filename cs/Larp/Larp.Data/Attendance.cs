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

    [Obsolete("Use Paid instead"), BsonIgnore]
    public decimal? ProvidedPayment { get; set; }

    [Obsolete("Use Cost instead"), BsonIgnore]
    public decimal? ExpectedPayment { get; set; }

    [BsonIgnoreIfNull] public int? Paid { get; set; }

    [BsonIgnoreIfNull] public int? Cost { get; set; }

    public MwFifthAttendance? MwFifth { get; set; }
}