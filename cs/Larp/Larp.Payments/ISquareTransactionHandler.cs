namespace Larp.Payments;

public interface ISquareTransactionHandler
{
    Task PaymentCreated(string paymentId);
    Task PaymentUpdated(string paymentId);
    Task OrderCreated(string orderId);
    Task OrderUpdated(string orderId);
    Task RefundCreated(string refundId);
    Task RefundUpdated(string refundId);
    Task PointOfSaleComplete(string transactionId);
}