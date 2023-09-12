using Square.Models;

namespace Larp.Payments;

public interface ISquareTransactionHandler
{
    Task PaymentCreated(Payment payment);
    Task PaymentUpdated(Payment payment);
}