using Larp.Payments;

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

    public async Task PaymentCreated(string paymentId)
    {
        await _transactionManager.UpdatePayment(paymentId);
    }

    public async Task PaymentUpdated(string paymentId)
    {
        await _transactionManager.UpdatePayment(paymentId);
    }

    public async Task OrderCreated(string orderId)
    {
        await _transactionManager.UpdateOrder(orderId);
    }

    public async Task OrderUpdated(string orderId)
    {
        await _transactionManager.UpdateOrder(orderId);
    }

    public async Task PointOfSaleComplete(string transactionId)
    {
        await _transactionManager.UpdateOrder(transactionId);
    }
}