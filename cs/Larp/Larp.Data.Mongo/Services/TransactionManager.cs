using Larp.Square;

namespace Larp.Data.Mongo.Services;

public class TransactionManager
{
    private readonly LarpContext _larpContext;
    private readonly ISquareService _squareService;

    public TransactionManager(LarpContext larpContext, ISquareService squareService)
    {
        _larpContext = larpContext;
        _squareService = squareService;
    }

    public async Task<string> RequestPayment(string accountId, decimal amount, string name, string email, string phone)
    {
        var id = ObjectId.GenerateNewId().ToString();
        var response = await _squareService.CreatePaymentUrl(id, amount, "Tavern Deposit", name, email, phone);

        var transaction = new Transaction()
        {
            TransactionId = id,
            AccountId = accountId,
            Type = TransactionType.Deposit,
            Source = "Square",
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
            .SumAsync(transaction => transaction.Amount);
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
            .ToDictionary(t => t.AccountId, t => t.Balance);
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