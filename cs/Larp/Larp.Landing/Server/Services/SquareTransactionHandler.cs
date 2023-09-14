using System.Text.Json;
using Larp.Payments;
using Square.Models;
using Transaction = Larp.Data.Transaction;

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

        var status = Transaction.ConvertTransactionStatus(payment.Status);

        await _transactionManager.UpdateByOrderId(
            payment.OrderId,
            status,
            payment.TotalMoney.Amount ?? 0,
            DateTimeOffset.Parse(payment.UpdatedAt),
            payment.ReceiptUrl);
    }

    public Task PaymentUpdated(Payment payment) => PaymentCreated(payment);
}