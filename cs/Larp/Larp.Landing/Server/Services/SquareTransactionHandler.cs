using System.Text.Json;
using Larp.Square;
using Square.Models;

namespace Larp.Landing.Server.Services;

public class SquareTransactionHandler : ISquareTransactionHandler
{
    private readonly ILogger<SquareTransactionHandler> _logger;
    private readonly TransactionManager _transactionManager;

    public SquareTransactionHandler(TransactionManager transactionManager, ILogger<SquareTransactionHandler> logger)
    {
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task PaymentCreated(Payment payment)
    {
        _logger.LogInformation("Payment: {Json}",
            JsonSerializer.Serialize(payment, new JsonSerializerOptions() { WriteIndented = true }));

        var amount = (decimal)(payment.TotalMoney.Amount ?? 0) / 100;
        var status = payment.Status switch
        {
            "APPROVED" => TransactionStatus.Approved,
            "PENDING" => TransactionStatus.Pending,
            "COMPLETED" => TransactionStatus.Complete,
            "CANCELED" => TransactionStatus.Cancelled,
            "FAILED" => TransactionStatus.Failed,
            _ => TransactionStatus.Unknown
        };

        await _transactionManager.UpdateByOrderId(
            payment.OrderId,
            status,
            amount,
            DateTimeOffset.Parse(payment.UpdatedAt),
            payment.ReceiptUrl);
    }

    public Task PaymentUpdated(Payment payment) => PaymentCreated(payment);
}