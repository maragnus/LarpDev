using Larp.Common;
using Larp.Payments;
using Square.Models;

namespace Larp.Data.Mongo.Services;

public class TransactionManager
{
    private const string SquareSource = "Square";
    private const string DepositName = "Tavern Deposit";
    private readonly LarpContext _larpContext;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ISquareService _squareService;
    private readonly IUserSessionManager _userSessionManager;

    public TransactionManager(LarpContext larpContext, ISquareService squareService,
        ILogger<TransactionManager> logger, IUserSessionManager userSessionManager)
    {
        _larpContext = larpContext;
        _squareService = squareService;
        _logger = logger;
        _userSessionManager = userSessionManager;
    }

    private SiteAccount SiteAccountFromAccount(Account account) =>
        new SiteAccount
        {
            AccountId = account.AccountId,
            Email = account.PreferredEmail,
            Phone = account.Phone,
            BirthDate = account.BirthDate,
            FinancialAccess = account.Roles.Contains(AccountRole.FinanceAccess),
            CustomerId = account.SquareCustomerId
        }.WithFillName(account.Name);

    public async Task<string> GetAccountDeviceCode(string accountId)
    {
        var account = await _userSessionManager.GetUserAccount(accountId);

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

        // Update SquareCustomerId
        var updates = items
            .Where(a => !string.IsNullOrEmpty(a.CustomerId))
            .Select(account => new UpdateManyModel<Account>(
                Builders<Account>.Filter.Eq(a => a.AccountId, account.AccountId),
                Builders<Account>.Update.Set(a => a.SquareCustomerId, account.CustomerId)))
            .ToList();
        if (updates.Count > 0)
            await _larpContext.Accounts.BulkWriteAsync(updates);

        _userSessionManager.UserAccountChanged(items.Select(item => item.AccountId));

        _logger.LogInformation("Square: Updating Transactions");
        await SynchronizeOrders();
    }

    public async Task SynchronizeOnStartup()
    {
        await _squareService.Initialize();

        if (_squareService.SynchronizeOnStartup)
            await Synchronize();
    }

    private async Task SynchronizeOrders()
    {
        var orders = (await _squareService.GetOrders())
            .ToDictionary(p => p.Id);

        var payments = (await _squareService.GetPayments())
            .GroupBy(p => p.OrderId)
            .ToDictionary(p => p.Key, p => p.ToArray());

        var updates = new List<WriteModel<Transaction>>();
        foreach (var item in orders.Values)
        {
            var order = item;

            var customerId = order.CustomerId;
            if (customerId is null)
            {
                var sourceOrderId = order.Returns?.FirstOrDefault()?.SourceOrderId;
                if (sourceOrderId is not null)
                {
                    _logger.LogWarning("Checking Square Order {OrderId} Source Order {SourceOrderId} for Customer Id",
                        order.Id, sourceOrderId);

                    var sourceOrder = orders.GetValueOrDefault(sourceOrderId)
                                      ?? await _squareService.GetOrder(sourceOrderId);
                    customerId = sourceOrder?.CustomerId;
                }

                if (customerId is null)
                {
                    _logger.LogWarning("Ignoring Square Order {OrderId} with null Customer Id", order.Id);
                    continue;
                }
            }

            var account = await GetAccountFromCustomerId(customerId);
            if (account is null)
            {
                _logger.LogWarning(
                    "Ignoring Square Order {OrderId} where Customer Id {CustomerId} does not link to Account",
                    order.Id, order.CustomerId);
                continue;
            }

            var orderPayments = payments.GetValueOrDefault(order.Id) ?? Array.Empty<Payment>();

            if (order.State == "OPEN")
            {
                var paid = order.Tenders.Any(t => t.AmountMoney.Amount is > 0);
                _logger.LogInformation("Square Order {OrderId} Paid={IsPaid}",
                    order.Id, paid);

                if (paid)
                    order = await _squareService.CompleteOrder(order);
            }

            var update = new UpdateOneModel<Transaction>(
                Builders<Transaction>.Filter.Eq(t => t.OrderId, order.Id),
                BuildUpdateTransactionModel(order, account.AccountId, orderPayments))
            {
                IsUpsert = true
            };

            updates.Add(update);
        }

        if (updates.Count > 0)
            await _larpContext.Transactions.BulkWriteAsync(updates);
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
            .SumAsync(transaction => transaction.Amount ?? 0);

        var withdrawals = await _larpContext.Attendances.AsQueryable()
            .Where(transaction => transaction.AccountId == accountId && transaction.Cost.HasValue)
            .SumAsync(transaction => (transaction.Cost ?? 0) - (transaction.Paid ?? 0));

        return (deposits - withdrawals).ToCurrency();
    }

    public async Task<Dictionary<string, decimal?>> GetBalances()
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

    public async Task UpdateByOrderId(string orderId, TransactionStatus status, long? amount,
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

    private async Task<Account?> GetAccountFromCustomerId(string? customerId)
    {
        if (string.IsNullOrEmpty(customerId))
            return null;

        var account = await _larpContext.Accounts
            .Find(a => a.SquareCustomerId == customerId)
            .FirstOrDefaultAsync();
        if (account is not null)
            return account;

        // Cross-reference missing, do a reverse lookup with Square

        var customer =
            await _squareService.GetCustomer(customerId)
            ?? throw new BadRequestException("Invalid Customer Id on Payment");

        if (string.IsNullOrEmpty(customer.ReferenceId))
        {
            _logger.LogInformation(
                "Customer Id {CustomerId} does not have a Reference Id",
                customer.Id);
            return null;
        }

        account = await _larpContext.Accounts
            .Find(a => a.AccountId == customer.ReferenceId)
            .FirstOrDefaultAsync();

        if (account is null)
        {
            _logger.LogInformation(
                "Customer Id {CustomerId} with Reference Id {ReferenceId} does not reference an Account",
                customer.Id, customer.ReferenceId);
            return null;
        }

        // Store this value so it's faster next time
        await _larpContext.Accounts.UpdateOneAsync(
            a => a.AccountId == account.AccountId,
            Builders<Account>.Update
                .Set(a => a.SquareCustomerId, customerId));

        return account;
    }

    private async Task<Transaction?> GetTransactionByOrder(string orderId)
    {
        return await _larpContext.Transactions
            .Find(transaction => transaction.OrderId == orderId)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateOrder(string orderId)
    {
        var order = await _squareService.GetOrder(orderId);
        if (order is null) return;
        await UpdateOrder(order);
    }

    private async Task UpdateOrder(Order order)
    {
        // Get Customer ID from order or a linked order
        var customerId = order.CustomerId;
        if (customerId is null)
        {
            var sourceOrderId = order.Returns.FirstOrDefault()?.SourceOrderId;
            if (sourceOrderId is not null)
            {
                var sourceOrder = await _squareService.GetOrder(sourceOrderId);
                customerId = sourceOrder?.CustomerId;
            }

            if (customerId is null)
            {
                _logger.LogWarning("Ignoring Square Order {OrderId} with null Customer Id", order.Id);
                return;
            }
        }

        var account = await GetAccountFromCustomerId(order.CustomerId);
        if (account is null)
        {
            _logger.LogWarning(
                "Ignoring Square Order {OrderId} where Customer Id {CustomerId} does not link to Account",
                order.Id, order.CustomerId);
            return;
        }

        if (order.State == "OPEN")
        {
            var paid = order.Tenders.Any(t => t.AmountMoney.Amount is > 0);
            _logger.LogInformation("Square Order {OrderId} Paid={IsPaid}",
                order.Id, paid);

            if (paid)
                order = await _squareService.CompleteOrder(order);
        }

        await _larpContext.Transactions.UpdateOneAsync(
            transaction => transaction.OrderId == order.Id,
            BuildUpdateTransactionModel(order, account.AccountId),
            new UpdateOptions() { IsUpsert = true });
    }

    public async Task UpdatePayment(string paymentId)
    {
        var payment = await _squareService.GetPayment(paymentId);
        if (payment is null) return;
        await UpdatePayment(payment);
    }

    private async Task UpdatePayment(Payment payment)
    {
        if (payment.CustomerId is null)
        {
            _logger.LogWarning(
                "Ignoring Square Payment {PaymentId} on Order {OrderId} with null Customer Id",
                payment.Id, payment.OrderId);
            return;
        }

        var account = await GetAccountFromCustomerId(payment.CustomerId);
        if (account is null)
        {
            _logger.LogWarning(
                "Ignoring Square Payment {PaymentId} where Customer Id {CustomerId} does not link to Account",
                payment.Id, payment.CustomerId);
            return;
        }

        await _larpContext.Transactions.UpdateOneAsync(
            transaction => transaction.OrderId == payment.OrderId,
            Builders<Transaction>.Update
                .SetOnInsert(t => t.OrderId, payment.OrderId)
                .SetOnInsert(t => t.TransactionOn, DateTimeOffset.Parse(payment.CreatedAt))
                .Set(t => t.UpdatedOn, DateTimeOffset.Parse(payment.UpdatedAt))
                .SetOnInsert(t => t.AccountId, account.AccountId)
                .SetOnInsert(t => t.Source, SquareSource)
                .SetOnInsert(t => t.Status, TransactionStatus.Pending)
                .SetOnInsert(t => t.Type, TransactionType.Deposit)
                .AddToSet(t => t.ReceiptUrls, payment.ReceiptUrl),
            new UpdateOptions() { IsUpsert = true });
    }

    private UpdateDefinition<Transaction> BuildUpdateTransactionModel(Order order, string accountId,
        Payment[]? payments = null)
    {
        var state = order.State switch
        {
            "DRAFT" => TransactionStatus.Unpaid,
            "OPEN" => TransactionStatus.Pending,
            "COMPLETED" => TransactionStatus.Completed,
            "CANCELED" => TransactionStatus.Cancelled,
            _ => TransactionStatus.Unknown
        };

        var paidAmount =
            state == TransactionStatus.Unpaid
                ? null
                : order.TotalMoney?.Amount.ToInt32();

        var returnAmount =
            order.ReturnAmounts?.TotalMoney?.Amount.ToInt32();

        var amount =
            paidAmount.HasValue || returnAmount.HasValue
                ? (int?)((paidAmount ?? 0) - (returnAmount ?? 0))
                : null;

        var type = amount switch
        {
            < 0 => TransactionType.Refund,
            > 0 => TransactionType.Deposit,
            0 when order.Returns.Count > 0 => TransactionType.Refund,
            _ => TransactionType.Deposit
        };

        var model = Builders<Transaction>.Update
            .SetOnInsert(t => t.OrderId, order.Id)
            .SetOnInsert(t => t.TransactionOn, DateTimeOffset.Parse(order.CreatedAt))
            .Set(t => t.UpdatedOn, DateTimeOffset.Parse(order.UpdatedAt))
            .Set(t => t.AccountId, accountId)
            .SetOnInsert(t => t.Source, SquareSource)
            .SetOnInsert(t => t.Status, state)
            .Set(t => t.Amount, amount)
            .SetOnInsert(t => t.Type, type);

        if (payments is not { Length: > 0 }) return model;

        // Add receipt list
        var receipts = payments
            .Select(op => op.ReceiptUrl)
            .Where(url => !string.IsNullOrEmpty(url));
        return model.Set(t => t.ReceiptUrls, receipts);
    }

    public async Task<AccountName> VerifyLinkedAccount(string email)
    {
        var account =
            ObjectId.TryParse(email, out var id)
                ? await _userSessionManager.GetUserAccount(id.ToString()!)
                : await _userSessionManager.FindByEmail(email);

        if (account is null) return new AccountName();

        return new AccountName
        {
            AccountId = account.AccountId,
            Name = account.Name ?? "Unnamed Account",
            Emails = { new AccountEmail { Email = account.PreferredEmail! } }
        };
    }

    public async Task<AccountName[]> GetLinkedAccounts(string accountId)
    {
        var query =
            from transaction in _larpContext.Transactions.AsQueryable()
            join account in _larpContext.Accounts.AsQueryable() on transaction.SourceAccountId equals account.AccountId
            select account;
        var accounts = await query.ToListAsync();

        return accounts
            .DistinctBy(account => account.AccountId)
            .Where(account => account.AccountId != accountId)
            .Select(account => new AccountName
            {
                AccountId = account.AccountId,
                Name = account.Name ?? "Unnamed Account",
                Emails = { new AccountEmail { Email = account.PreferredEmail! } }
            }).ToArray();
    }

    public async Task Transfer(string fromAccountId, string toAccountId, int amount)
    {
        if (fromAccountId == toAccountId)
            throw new BadRequestException("Cannot transfer to the same account");

        await _userSessionManager.GetUserAccount(fromAccountId);
        await _userSessionManager.GetUserAccount(toAccountId);

        var balance = await GetBalance(fromAccountId);

        if (amount > balance.ToCents())
        {
            _logger.LogWarning(
                "Transfer from Account {FromAccountId} to Account {ToAccountId} of {Amount} cannot complete because it has insufficient funds of {Balance}",
                fromAccountId, toAccountId, amount, balance);
            throw new BadRequestException($"Insufficient funds");
        }

        var fromTransaction = new Transaction
        {
            AccountId = fromAccountId,
            TransactionOn = DateTimeOffset.Now,
            Type = TransactionType.Withdrawal,
            Status = TransactionStatus.Completed,
            Amount = -amount,
            Source = "Tavern Transfer",
            UpdatedOn = DateTimeOffset.Now,
            SourceAccountId = toAccountId
        };

        var toTransaction = new Transaction
        {
            AccountId = toAccountId,
            TransactionOn = DateTimeOffset.Now,
            Type = TransactionType.Deposit,
            Status = TransactionStatus.Completed,
            Amount = amount,
            Source = "Tavern Transfer",
            UpdatedOn = DateTimeOffset.Now,
            SourceAccountId = fromAccountId
        };

        await _larpContext.Transactions.InsertOneAsync(fromTransaction);
        await _larpContext.Transactions.InsertOneAsync(toTransaction);
    }
}