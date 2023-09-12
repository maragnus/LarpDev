namespace Larp.Data;

public enum TransactionType
{
    Unknown,
    Deposit,
    Withdrawal,
}

public class Transaction
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? TransactionId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? AccountId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? SourceAccountId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? EventId { get; set; }

    public string? Source { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public DateTimeOffset TransactionOn { get; set; }

    public string? SquareId { get; set; }

    public string? Note { get; set; }

    public string? AdminNotes { get; set; }

    [BsonIgnore] public string? SourceAccountName { get; set; }

    [BsonIgnore] public string? EventTitle { get; set; }
}