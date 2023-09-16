namespace Larp.Payments;

public interface ISquareTransactionHandler
{
    Task PaymentCreated(string paymentId);
    Task PaymentUpdated(string paymentId);
    Task OrderCreated(string orderId);
    Task OrderUpdated(string orderId);
    Task PointOfSaleComplete(string transactionId);
}