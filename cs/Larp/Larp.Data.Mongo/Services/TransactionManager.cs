using Larp.Common;
using Larp.Payments;

namespace Larp.Data.Mongo.Services;

public class TransactionManager
{
    private const string SquareSource = "Square";
    private const string SquareOnSiteSource = "Square On-Site";
    private const string DepositName = "Tavern Deposit";
    private readonly LarpContext _larpContext;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ISquareService _squareService;

    public TransactionManager(LarpContext larpContext, ISquareService squareService, ILogger<TransactionManager> logger)
    {
        _larpContext = larpContext;
        _squareService = squareService;
        _logger = logger;
    }

    private SiteAccount SiteAccountFromAccount(Account account) =>
        new SiteAccount
        {
            AccountId = account.AccountId,
            Email = account.PreferredEmail,
            Phone = account.Phone,
            BirthDate = account.BirthDate,
            FinancialAccess = account.Roles.Contains(AccountRole.FinanceAccess),
        }.WithFillName(account.Name);

    public async Task<string> GetAccountDeviceCode(string accountId)
    {
        var account = await _larpContext.Accounts
                          .Find(account => account.AccountId == accountId)
                          .FirstOrDefaultAsync()
                      ?? throw new BadRequestException("Account not found");

        if (!string.IsNullOrEmpty(account.SquareDeviceCode))
            return account.SquareDeviceCode;

        var code = await _squareService.GenerateDeviceCode($"{account.Name} {account.PreferredEmail}");
        await _larpContext.Accounts.UpdateOneAsync(
            a => a.AccountId == accountId,
            Builders<Account>.Update.Set(a => a.SquareDeviceCode, code));
        account.SquareDeviceCode = code;

        return account.SquareDeviceCode;
    }

    public async Task Synchronize()
    {
        var accounts = await _larpContext.Accounts
            .Find(account => account.State == AccountState.Active && account.FirstLogin.HasValue)
            .ToListAsync();

        var items = accounts.Select(SiteAccountFromAccount)
            .ToArray();

        await _squareService.Synchronize(items);

        _logger.LogInformation("Square: Updating Transactions");
        await UpdatePayments();
    }

    public async Task SynchronizeOnStartup()
    {
        if (_squareService.SynchronizeOnStartup)
            await Synchronize();
        else
            await _squareService.Initialize();
    }

    private async Task UpdatePayments()
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

            var amount = payment.TotalMoney.Amount ?? 0;
            var status = Transaction.ConvertTransactionStatus(payment.Status);
            var updatedAt = DateTimeOffset.Parse(payment.UpdatedAt);

            if (transaction.Status == status
                && transaction.TransactionOn == updatedAt
                && transaction.Amount == amount
                && transaction.ReceiptUrl == payment.ReceiptUrl)
                continue;

            _logger.LogInformation("Updating transaction: {Id} {Amount} {Date}", transaction.TransactionId,
                transaction.AmountDecimal, updatedAt);

            var write = new UpdateOneModel<Transaction>(
                Builders<Transaction>.Filter.Eq(t => t.TransactionId, transaction.TransactionId),
                Builders<Transaction>.Update
                    .Set(t => t.Status, status)
                    .Set(t => t.Amount, (int)amount)
                    .Set(t => t.ReceiptUrl, payment.ReceiptUrl)
                    .Set(t => t.TransactionOn, updatedAt)
            );

            writes.Add(write);
        }

        if (writes.Count == 0) return;

        await _larpContext.Transactions.BulkWriteAsync(writes);
    }

    public async Task<string> RequestPayment(string accountId, decimal amount, Account account)
    {
        var id = ObjectId.GenerateNewId().ToString()!;
        var response = await _squareService.CreatePaymentUrl(id, amount, DepositName, SiteAccountFromAccount(account));

        var transaction = new Transaction()
        {
            TransactionId = id,
            AccountId = accountId,
            Type = TransactionType.Deposit,
            Source = SquareSource,
            OrderId = response.OrderId,
            OrderUrl = response.LongUrl,
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

        var attendances = await _larpContext.Attendances
            .Find(attendance => attendance.AccountId == accountId && attendance.Cost.HasValue)
            .ToListAsync();

        var eventIds = transactions.Select(transaction => transaction.EventId)
            .Concat(attendances.Select(a => a.EventId))
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToArray();

        var events =
            eventIds.Length == 0
                ? new Dictionary<string, (string Title, DateTimeOffset Date)>()
                : (await _larpContext.Events
                    .Find(Builders<Event>.Filter.In(@event => @event.EventId, eventIds))
                    .Project<Event>(Builders<Event>.Projection
                        .Include(e => e.EventId)
                        .Include(e => e.Title)
                        .Include(e => e.Date)
                    )
                    .ToListAsync())
                .ToDictionary(
                    a => a.EventId,
                    a => (a.Title ?? "Untitled Event", new DateTimeOffset(a.Date.ToDateTime(TimeOnly.MinValue))));

        transactions.AddRange(
            attendances.Select(attendance =>
            {
                var @event = events.GetValueOrDefault(attendance.EventId);
                return new Transaction
                {
                    AccountId = attendance.AccountId,
                    Amount = -1 * Math.Max(0, (attendance.Cost ?? 0) - (attendance.Paid ?? 0)),
                    EventId = attendance.EventId,
                    EventTitle = @event.Item1,
                    TransactionOn = @event.Item2,
                    Status = TransactionStatus.Completed,
                    Type = TransactionType.Withdrawal
                };
            })
        );

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

        foreach (var transaction in transactions)
        {
            transaction.SourceAccountName = transaction.SourceAccountId is null
                ? null
                : accounts.GetValueOrDefault(transaction.SourceAccountId);

            transaction.EventTitle = transaction.EventId is null
                ? null
                : events.GetValueOrDefault(transaction.EventId).Item1;
        }

        return transactions.ToArray();
    }

    public async Task<decimal> GetBalance(string accountId)
    {
        var deposits = await _larpContext.Transactions.AsQueryable()
            .Where(transaction => transaction.AccountId == accountId)
            .SumAsync(transaction => transaction.Amount);

        var withdrawals = await _larpContext.Attendances.AsQueryable()
            .Where(transaction => transaction.AccountId == accountId && transaction.Cost.HasValue)
            .SumAsync(transaction => transaction.Cost - (transaction.Paid ?? 0));

        return (deposits - (withdrawals ?? 0)).ToCurrency();
    }

    public async Task<Dictionary<string, decimal>> GetBalances()
    {
        var deposits =
            (await _larpContext.Transactions.Aggregate()
                .Group(
                    transaction => transaction.AccountId,
                    group => new
                    {
                        AccountId = group.Key!,
                        Balance = group.Sum(transaction => transaction.Amount)
                    })
                .ToListAsync())
            .ToDictionary(t => t.AccountId, t => t.Balance.ToCurrency());

        var withdrawals =
            (await _larpContext.Attendances.Aggregate()
                .Group(
                    attendance => attendance.AccountId,
                    group => new
                    {
                        AccountId = group.Key!,
                        Cost = group.Sum(attendance => attendance.Cost),
                        Paid = group.Sum(attendance => attendance.Paid)
                    })
                .ToListAsync())
            .ToDictionary(t => t.AccountId, t =>
                (t.Cost.ToCurrency() ?? 0) - (t.Paid.ToCurrency() ?? 0));

        foreach (var transaction in deposits.ToList())
        {
            if (withdrawals.TryGetValue(transaction.Key, out var withdrawal))
                deposits[transaction.Key] -= withdrawal;
        }

        return deposits;
    }

    public async Task UpdateByOrderId(string orderId, TransactionStatus status, long amount,
        DateTimeOffset timestamp, string? receiptUrl)
    {
        var update = Builders<Transaction>.Update
            .Set(t => t.Status, status)
            .Set(t => t.Amount, amount)
            .Set(t => t.TransactionOn, timestamp)
            .Set(t => t.UpdatedOn, DateTimeOffset.Now)
            .Set(t => t.ReceiptUrl, receiptUrl);

        // Prevent reverting status from out-of-order requests
        if (status == TransactionStatus.Completed)
            await _larpContext.Transactions.UpdateOneAsync(t => t.OrderId == orderId, update);
        else
            await _larpContext.Transactions.UpdateOneAsync(
                t => t.OrderId == orderId && t.Status != TransactionStatus.Completed, update);
    }

    public async Task<string> PointOfSale(string accountId, int amount, DeviceType deviceType)
    {
        var account = await _larpContext.Accounts.Find(a => a.AccountId == accountId)
                          .FirstOrDefaultAsync()
                      ?? throw new BadRequestException("Account could not be found");

        var id = ObjectId.GenerateNewId().ToString()!;
        var response =
            await _squareService.CreatePointOfSale(id, amount, DepositName, SiteAccountFromAccount(account),
                deviceType);

        return response.Url;
    }

    public async Task ImportTransaction(string squareTransactionId)
    {
        var order = await _squareService.GetOrder(squareTransactionId);
        var transaction = await _larpContext.Transactions
            .Find(t => t.OrderId == squareTransactionId)
            .FirstOrDefaultAsync();

        var amount = (int)(order.TotalMoney.Amount ?? 0);
        var status = Transaction.ConvertTransactionStatus(order.State);

        if (transaction == null)
        {
            var customer = await _squareService.GetCustomer(order.CustomerId);
            var updatedAt = DateTimeOffset.Parse(order.UpdatedAt);
            var transactionId = ObjectId.GenerateNewId().ToString();

            await _larpContext.Transactions.InsertOneAsync(new Transaction()
            {
                TransactionId = transactionId,
                AccountId = customer.ReferenceId,
                Amount = amount,
                Status = status,
                TransactionOn = updatedAt,
                UpdatedOn = updatedAt,
                OrderId = order.Id,
                Source = SquareOnSiteSource,
                Type = TransactionType.Deposit
            });
        }
    }
}