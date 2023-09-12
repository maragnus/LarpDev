namespace Larp.Data;

public enum TransactionType
{
    Unknown,
    Deposit,
    Withdrawal,
}

public enum TransactionStatus
{
    Unknown,
    Completed,
    Pending,
    Approved,
    Cancelled,
    Failed
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

    public TransactionStatus Status { get; set; }

    public string? Source { get; set; }

    public TransactionType Type { get; set; }

    public long Amount { get; set; }

    [BsonIgnore] public decimal AmountDecimal => Amount / 100m;

    public DateTimeOffset TransactionOn { get; set; }

    public string? OrderId { get; set; }

    public string? Note { get; set; }

    public string? AdminNotes { get; set; }

    public string? ReceiptUrl { get; set; }

    [BsonIgnore] public string? SourceAccountName { get; set; }

    [BsonIgnore] public string? EventTitle { get; set; }

    public static TransactionStatus ConvertTransactionStatus(string status) =>
        status.ToUpperInvariant() switch
        {
            "APPROVED" => TransactionStatus.Approved,
            "PENDING" => TransactionStatus.Pending,
            "COMPLETED" => TransactionStatus.Completed,
            "CANCELED" => TransactionStatus.Cancelled,
            "FAILED" => TransactionStatus.Failed,
            _ => TransactionStatus.Unknown
        };
}