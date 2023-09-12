using Square.Models;

namespace Larp.Square;

public interface ISquareTransactionHandler
{
    Task PaymentCreated(Payment payment);
    Task PaymentUpdated(Payment payment);
}