using Larp.Payments;

namespace Larp.Data.Mongo.Services;

public class TransactionManager
{
    public const string SquareSource = "Square";
    public const string DepositName = "Tavern Deposit";
    private readonly LarpContext _larpContext;
    private readonly ISquareService _squareService;

    public TransactionManager(LarpContext larpContext, ISquareService squareService)
    {
        _larpContext = larpContext;
        _squareService = squareService;
    }

    public async Task UpdatePayments()
    {
        var payments = (await _squareService.GetPayments())
            .ToDictionary(p => p.OrderId);

        var transactions = await _larpContext.Transactions
            .Find(t => t.Source == SquareSource)
            .ToListAsync();

        var writes = new List<WriteModel<Transaction>>();

        foreach (var transaction in transactions)
        {
            if (string.IsNullOrEmpty(transaction.OrderId)
                || !payments.TryGetValue(transaction.OrderId, out var payment))
                continue;

            var amount = payment.TotalMoney.Amount;
            var status = Transaction.ConvertTransactionStatus(payment.Status);
            var updatedAt = DateTimeOffset.Parse(payment.UpdatedAt);
            if (transaction.Status == status
                && transaction.TransactionOn == updatedAt
                && transaction.Amount == amount
                && transaction.ReceiptUrl == payment.ReceiptUrl)
                continue;

            var write = new UpdateOneModel<Transaction>(
                Builders<Transaction>.Filter.Eq(t => t.TransactionId, transaction.TransactionId),
                Builders<Transaction>.Update
                    .Set(t => t.Status, status)
                    .Set(t => t.Amount, amount)
                    .Set(t => t.ReceiptUrl, payment.ReceiptUrl)
                    .Set(t => t.TransactionOn, updatedAt)
            );

            writes.Add(write);
        }

        if (writes.Count == 0) return;

        await _larpContext.Transactions.BulkWriteAsync(writes);
    }

    public async Task<string> RequestPayment(string accountId, decimal amount, string? name, string? email,
        string? phone)
    {
        var id = ObjectId.GenerateNewId().ToString();
        var response = await _squareService.CreatePaymentUrl(id, amount, DepositName, name, email, phone);

        var transaction = new Transaction()
        {
            TransactionId = id,
            AccountId = accountId,
            Type = TransactionType.Deposit,
            Source = SquareSource,
            OrderId = response.OrderId,
            Status = TransactionStatus.Pending,
            TransactionOn = DateTimeOffset.Now
        };
        await _larpContext.Transactions.InsertOneAsync(transaction);
        return response.Url;
    }

    public async Task<Transaction[]> GetTransactions(string accountId)
    {
        var transactions =
            await _larpContext.Transactions
                .Find(transaction => transaction.AccountId == accountId)
                .ToListAsync();

        var accountIds = transactions.Select(transaction => transaction.AccountId)
            .Concat(transactions.Select(transaction => transaction.SourceAccountId))
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToArray();
        var accounts =
            accountIds.Length == 0
                ? new Dictionary<string, string>()
                : (await _larpContext.Accounts
                    .Find(Builders<Account>.Filter.In(account => account.AccountId, accountIds))
                    .ToListAsync())
                .ToDictionary(a => a.AccountId, a => a.Name ?? "Unnamed Account");

        var eventIds = transactions.Select(transaction => transaction.EventId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToArray();
        var events =
            eventIds.Length == 0
                ? new Dictionary<string, string>()
                : (await _larpContext.Events
                    .Find(Builders<Event>.Filter.In(@event => @event.EventId, eventIds))
                    .ToListAsync())
                .ToDictionary(a => a.EventId, a => a.Title ?? "Untitled Event");

        foreach (var transaction in transactions)
        {
            transaction.SourceAccountName = transaction.SourceAccountId is null
                ? null
                : accounts.GetValueOrDefault(transaction.SourceAccountId);

            transaction.EventTitle = transaction.EventId is null
                ? null
                : events.GetValueOrDefault(transaction.EventId);
        }

        return transactions.ToArray();
    }

    public async Task<decimal> GetBalance(string accountId)
    {
        return await _larpContext.Transactions.AsQueryable()
            .Where(transaction => transaction.AccountId == accountId)
            .SumAsync(transaction => transaction.Amount) / 100m;
    }

    public async Task<Dictionary<string, decimal>> GetBalances()
    {
        return
            (await _larpContext.Transactions.Aggregate()
                .Group(
                    transaction => transaction.AccountId,
                    group => new
                    {
                        AccountId = group.Key!,
                        Balance = group.Sum(transaction => transaction.Amount)
                    })
                .ToListAsync())
            .ToDictionary(t => t.AccountId, t => t.Balance / 100m);
    }

    public async Task UpdateByOrderId(string orderId, TransactionStatus status, decimal amount,
        DateTimeOffset timestamp, string? receiptUrl)
    {
        await _larpContext.Transactions.UpdateOneAsync(t => t.OrderId == orderId,
            Builders<Transaction>.Update
                .Set(t => t.Status, status)
                .Set(t => t.Amount, amount)
                .Set(t => t.TransactionOn, timestamp)
                .Set(t => t.ReceiptUrl, receiptUrl)
        );
    }
}