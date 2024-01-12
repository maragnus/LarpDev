using System.Text;
using System.Text.Json;
using KiloTx.Restful;
using Larp.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Square;
using Square.Exceptions;
using Square.Models;
using Environment = Square.Environment;

namespace Larp.Payments;

public interface ISquareService
{
    bool SynchronizeOnStartup { get; }

    Task<SquarePaymentUrl> CreatePaymentUrl(string transactionId, decimal amount, string itemName, SiteAccount account);

    /// <summary>
    /// Returns the <paramref name="limit"/> most recently created Payments
    /// </summary>
    /// <remarks>https://developer.squareup.com/reference/square/payments-api/list-payments</remarks>
    Task<Payment[]> GetPayments(int limit = 100);

    /// <summary>
    /// Returns the <paramref name="limit"/> most recently updated Orders
    /// </summary>
    /// <remarks>https://developer.squareup.com/reference/square/orders-api/search-orders</remarks>
    Task<Order[]> GetOrders(int limit = 100);

    /// <summary>
    /// Returns the <paramref name="limit"/> most recently updated Refunds
    /// </summary>
    /// <remarks>https://developer.squareup.com/reference/square/refunds-api/list-payment-refunds</remarks>
    Task<PaymentRefund[]> GetRefunds(int limit = 100);

    Task Initialize();

    Task Synchronize(SiteAccount[] accounts);

    Task<SquarePaymentUrl> CreatePointOfSale(string id, int amount, string itemName, SiteAccount account,
        DeviceType deviceType);

    Task<string> GenerateDeviceCode(string name);

    Task<Order> CompleteOrder(Order order);

    Task<Order?> GetOrder(string orderId);

    Task<Customer?> GetCustomer(string customerId);
    Task<Payment?> GetPayment(string paymentId);
    Task<PaymentRefund?> GetRefund(string refundId);
}

public class SquareService : ISquareService
{
    private const bool PayOrderOnComplete = false;
    private static string? _locationId;

    private static readonly string[] WebhookEventTypes =
    {
        "payment.created", "payment.updated",
        "order.created", "order.updated",
        "refund.created", "refund.updated"
    };

    private readonly SquareClient _client;
    private readonly ILogger<SquareService> _logger;
    private readonly SquareOptions _options;
    
    public bool IsDisabled { get; }
    
    public SquareService(IOptions<SquareOptions> options, ILogger<SquareService> logger)
    {
        _logger = logger;
        _options = options.Value;
        IsDisabled = "disabled".Equals(_options.AccessToken); 
        if (!IsDisabled)
            _options.Validate();
        _client = CreateClient();
        SynchronizeOnStartup = _options.SynchronizeOnStartup;
    }

    public static string? SignatureKey { get; private set; }

    public bool SynchronizeOnStartup { get; }

    public async Task Initialize()
    {
        if (IsDisabled) return;
        
        try
        {
            _logger.LogInformation("Square: Updating Webhook Subscription");
            await UpdateWebhookSubscription();

            _logger.LogInformation("Square: Updating Default Location");
            await GetDefaultLocationId();
        }
        catch (ApiException ex)
        {
            _logger.LogInformation("Initialize failed: {Json}",
                ex.HttpContext.Response.Body);
            throw;
        }
    }

    public async Task Synchronize(SiteAccount[] accounts)
    {
        if (IsDisabled) return;

        try
        {
            _logger.LogInformation("Square: Updating Team Members");
            await SynchronizeTeamMembers(accounts);

            _logger.LogInformation("Square: Updating Customers");
            await SynchronizeCustomers(accounts);
        }
        catch (ApiException ex)
        {
            _logger.LogInformation("Synchronize failed: {Json}",
                ex.HttpContext.Response.Body);
            throw;
        }
    }

    public async Task<string> GenerateDeviceCode(string name)
    {
        if (IsDisabled) return "";

        var response = await _client.DevicesApi.CreateDeviceCodeAsync(
            new CreateDeviceCodeRequest(
                SquareUtilities.CreateIdempotencyKey(),
                new DeviceCode.Builder("TERMINAL_API")
                    .Name(name)
                    .LocationId(await GetDefaultLocationId())
                    .Build()));
        return response.DeviceCode.Code;
    }

    public async Task<Order?> GetOrder(string orderId)
    {
        var transaction = await _client.OrdersApi.RetrieveOrderAsync(orderId);
        return transaction.Order;
    }

    public async Task<Customer?> GetCustomer(string customerId)
    {
        var response = await _client.CustomersApi.RetrieveCustomerAsync(customerId);
        return response.Customer;
    }

    public async Task<Payment?> GetPayment(string paymentId)
    {
        var response = await _client.PaymentsApi.GetPaymentAsync(paymentId);
        return response.Payment;
    }

    public async Task<PaymentRefund?> GetRefund(string refundId)
    {
        var response = await _client.RefundsApi.GetPaymentRefundAsync(refundId);
        return response.Refund;
    }

    public async Task<Order> CompleteOrder(Order order)
    {
        if (order.Fulfillments.Count <= 0) return order;

        var orderModel = new Order.Builder(await GetDefaultLocationId())
            .State("COMPLETED")
            .Version(order.Version);

        var fulfillment = order.Fulfillments.FirstOrDefault();
        if (fulfillment is { State: not "COMPLETED" })
        {
            orderModel.Fulfillments(new[]
            {
                new Fulfillment.Builder()
                    .Uid(fulfillment.Uid)
                    .State("COMPLETED")
                    .Build()
            });
        }

        var response = await _client.OrdersApi.UpdateOrderAsync(
            order.Id,
            new UpdateOrderRequest(orderModel.Build()));
        return response.Order;
    }

    public async Task<SquarePaymentUrl> CreatePointOfSale(string id, int amount, string itemName, SiteAccount account,
        DeviceType deviceType)
    {
        var customer = await UpdateCustomer(account);

        if (deviceType == DeviceType.AndroidMobile)
        {
            var sb = new StringBuilder();

            sb.Append("intent:#Intent;").Append("action=com.squareup.pos.action.CHARGE;")
                .Append("package=com.squareup;").Append("S.browser_fallback_url=").Append(_options.ReturnUrl)
                .Append(';')
                .Append("S.com.squareup.pos.WEB_CALLBACK_URI=").Append(_options.PointOfSale.CallbackUrl).Append(';')
                .Append("S.com.squareup.pos.CLIENT_ID=").Append(_options.ApplicationId).Append(';')
                .Append("S.com.squareup.pos.API_VERSION=v2.0;")
                .Append("i.com.squareup.pos.TOTAL_AMOUNT=").Append(amount).Append(';')
                .Append("l.com.squareup.pos.AUTO_RETURN_TIMEOUT_MS=5000;")
                .Append("S.com.squareup.pos.CURRENCY_CODE=USD;")
                .Append("S.com.squareup.pos.CUSTOMER_ID=").Append(customer.Id).Append(';')
                .Append("S.com.squareup.pos.TENDER_TYPES=" +
                        "com.squareup.pos.TENDER_CARD," +
                        "com.squareup.pos.TENDER_CARD_ON_FILE," +
                        "com.squareup.pos.TENDER_CASH," +
                        "com.squareup.pos.TENDER_OTHER;")
                .Append("end");

            return new SquarePaymentUrl
            {
                Url = sb.ToString()
            };
        }

        if (deviceType == DeviceType.AppleMobile)
        {
            var sb = new StringBuilder();
            var request = new SquarePointOfSaleRequest()
            {
                ClientId = _options.ApplicationId,
                CustomerId = customer.Id,
                CallbackUrl = _options.PointOfSale.CallbackUrl,
                Version = "1.3",
                AutoReturn = true,
                AmountMoney =
                {
                    Amount = amount.ToString(),
                    CurrencyCode = "USD"
                },
                Options =
                {
                    SupportedTenderTypes = new[]
                    {
                        "CREDIT_CARD",
                        "CASH",
                        "OTHER",
                        "SQUARE_GIFT_CARD",
                        "CARD_ON_FILE"
                    }
                }
            };

            var json = JsonSerializer.Serialize(request);

            sb.Append("square-commerce-v1://payment/create?data=")
                .Append(Uri.EscapeDataString(json));

            return new SquarePaymentUrl
            {
                Url = sb.ToString()
            };
        }

        throw new BadRequestException("Unsupported device");
    }

    public async Task<Payment[]> GetPayments(int limit) =>
        await SquareUtilities.ListAsync(
            async () => await _client.PaymentsApi.ListPaymentsAsync(limit: limit, sortOrder: "DESC"),
            async (cursor) => await _client.PaymentsApi.ListPaymentsAsync(cursor),
            response => response.Payments,
            response => response.Cursor
        );

    /// <summary>
    /// Returns the <paramref name="limit"/> most recently updated Orders
    /// </summary>
    /// <param name="limit">Maximum number of Orders to return</param>
    public async Task<Order[]> GetOrders(int limit = 100) =>
        await SquareUtilities.ListAsync(
            async () => await _client.OrdersApi
                .SearchOrdersAsync(
                    new SearchOrdersRequest(new[] { await GetDefaultLocationId() },
                        null,
                        new SearchOrdersQuery(new SearchOrdersFilter(), new SearchOrdersSort("UPDATED_AT", "DESC")),
                        limit)),
            async (cursor) => await _client.OrdersApi.SearchOrdersAsync(new SearchOrdersRequest(null, cursor)),
            response => response.Orders,
            response => response.Cursor
        );

    public async Task<SquarePaymentUrl> CreatePaymentUrl(string transactionId, decimal amount, string itemName,
        SiteAccount account)
    {
        try
        {
            var customer = await UpdateCustomer(account);

            var locationId = await GetDefaultLocationId();

            var money = new Money.Builder()
                .Amount(amount.ToCents())
                .Currency("USD")
                .Build();

            var prepopulatedData = new PrePopulatedData.Builder()
                .BuyerAddress(new Address.Builder()
                    .FirstName(account.FirstName)
                    .LastName(account.LastName)
                    .Build())
                .BuyerEmail(account.Email);

            var order = new Order.Builder(locationId)
                .CustomerId(customer.Id)
                .ReferenceId(transactionId)
                .LineItems(new[]
                {
                    new OrderLineItem.Builder("1")
                        .Name(itemName)
                        .BasePriceMoney(money)
                        .Build()
                })
                .Build();

            if (SquareUtilities.TryFixPhoneNumber(account.Phone, out var phone))
                prepopulatedData.BuyerPhoneNumber(phone);

            var body = new CreatePaymentLinkRequest.Builder()
                .IdempotencyKey(SquareUtilities.CreateIdempotencyKey())
                .PaymentNote($"{customer.GivenName} {customer.FamilyName} {customer.EmailAddress}".Trim())
                .PrePopulatedData(prepopulatedData.Build())
                .Order(order)
                .CheckoutOptions(new CheckoutOptions.Builder()
                    .RedirectUrl(_options.ReturnUrl)
                    .AllowTipping(false)
                    .EnableCoupon(false)
                    .Build())
                .Build();

            var response = await _client.CheckoutApi.CreatePaymentLinkAsync(body);

            return new SquarePaymentUrl
            {
                OrderId = response.PaymentLink.OrderId,
                Url = response.PaymentLink.Url,
                LongUrl = response.PaymentLink.LongUrl
            };
        }
        catch (ApiException ex)
        {
            _logger.LogInformation("CreatePaymentLink failed: {Json}",
                ex.HttpContext.Response.Body);
            throw;
        }
    }

    public async Task<PaymentRefund[]> GetRefunds(int limit = 100) =>
        await SquareUtilities.ListAsync(
            async () => await _client.RefundsApi.ListPaymentRefundsAsync(limit: limit, sortOrder: "DESC"),
            async (cursor) => await _client.RefundsApi.ListPaymentRefundsAsync(cursor),
            response => response.Refunds,
            response => response.Cursor
        );

    private SquareClient CreateClient() =>
        new SquareClient.Builder()
            .AccessToken(_options.AccessToken)
            .SquareVersion("2023-08-16")
            .Environment(
                Enum.TryParse<Environment>(_options.Environment, out var env)
                    ? env
                    : Environment.Sandbox)
            .Build();

    private async Task UpdateWebhookSubscription()
    {
        var subscriptions = await GetWebhookSubscriptions();
        var subscription = subscriptions
            .FirstOrDefault(subscription => subscription.NotificationUrl == _options.Webhook.CallbackUrl);

        if (subscription is null)
        {
            var response = await _client.WebhookSubscriptionsApi.CreateWebhookSubscriptionAsync(
                new CreateWebhookSubscriptionRequest.Builder(new WebhookSubscription.Builder()
                        .Name(_options.Webhook.Name)
                        .NotificationUrl(_options.Webhook.CallbackUrl)
                        .Enabled(true)
                        .EventTypes(WebhookEventTypes)
                        .Build())
                    .IdempotencyKey(SquareUtilities.CreateIdempotencyKey())
                    .Build());
            SignatureKey = response.Subscription.SignatureKey;
            _logger.LogInformation("Square: Created Webhook Subscription");
            return;
        }

        if (!subscription.EventTypes.Order().SequenceEqual(WebhookEventTypes.Order()))
        {
            await _client.WebhookSubscriptionsApi.UpdateWebhookSubscriptionAsync(
                subscription.Id,
                new UpdateWebhookSubscriptionRequest(new WebhookSubscription.Builder()
                    .EventTypes(WebhookEventTypes)
                    .Build()));
        }

        var key = await _client.WebhookSubscriptionsApi
            .RetrieveWebhookSubscriptionAsync(subscription.Id);
        SignatureKey = key.Subscription.SignatureKey;
        _logger.LogInformation("Square: Received Webhook Subscription");
    }

    private async Task SynchronizeTeamMembers(SiteAccount[] siteAccounts)
    {
        var teamMembers = await GetTeamMembers();
        var accounts = siteAccounts
            .Select(account => new
            {
                Account = account,
                Square = teamMembers.FirstOrDefault(member => member.ReferenceId == account.AccountId)
                         ?? teamMembers.FirstOrDefault(member => string.Equals(member.EmailAddress, account.Email,
                             StringComparison.InvariantCultureIgnoreCase))
            })
            .ToList();

        foreach (var item in accounts)
        {
            var account = item.Account;
            var square = item.Square;

            try
            {
                if (account.FinancialAccess)
                    await UpdateTeamMember(account, square);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Square: Failed to update team member {FirstName} {LastName} {Email}: {Message}",
                    item.Account.FirstName,
                    item.Account.LastName,
                    item.Account.Email,
                    ex.Message);
            }
        }
    }

    private async Task SynchronizeCustomers(SiteAccount[] siteAccounts)
    {
        var customers = await GetCustomers();
        var accounts = customers
            .Join(siteAccounts, customer => customer.ReferenceId, account => account.AccountId, (member, account) => new
            {
                Square = (Customer?)member,
                Account = account
            })
            .Concat(
                siteAccounts
                    .ExceptBy(customers.Select(member => member.ReferenceId), account => account.AccountId)
                    .Select(account => new
                    {
                        Square = (Customer?)null,
                        Account = account
                    })
            )
            .ToList();

        foreach (var item in accounts)
        {
            try
            {
                await UpdateCustomer(item.Account, item.Square);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Square: Failed to update customer {FirstName} {LastName} {Email}: {Message}",
                    item.Account.FirstName,
                    item.Account.LastName,
                    item.Account.Email,
                    ex.Message);
            }
        }
    }

    private async Task<Customer?> FindCustomer(string accountReferenceId)
    {
        var filter = new CustomerFilter.Builder()
            .ReferenceId(new CustomerTextFilter(accountReferenceId))
            .Build();

        var body = new SearchCustomersRequest.Builder()
            .Query(new CustomerQuery(filter))
            .Build();

        var results = await _client.CustomersApi.SearchCustomersAsync(body);

        return results.Customers.FirstOrDefault();
    }

    private async Task<Customer> UpdateCustomer(SiteAccount account)
    {
        var customer = await FindCustomer(account.AccountId!);
        return await UpdateCustomer(account, customer);
    }

    private async Task<Customer> UpdateCustomer(SiteAccount account, Customer? square)
    {
        var hasPhone = SquareUtilities.TryFixPhoneNumber(account.Phone, out var phone);

        if (square == null)
        {
            var body = new CreateCustomerRequest.Builder()
                .IdempotencyKey(SquareUtilities.CreateIdempotencyKey())
                .GivenName(account.FirstName)
                .FamilyName(account.LastName)
                .ReferenceId(account.AccountId)
                .EmailAddress(account.Email)
                .ReferenceId(account.AccountId);
            if (account.BirthDate.HasValue)
                body.Birthday(account.BirthDate.Value.ToString("yyyy-MM-dd"));
            if (hasPhone)
                body.PhoneNumber(phone);

            var response = await _client.CustomersApi.CreateCustomerAsync(body.Build());
            _logger.LogInformation("Square: Created Customer {Email}", account.Email);

            account.CustomerId = response.Customer.Id;
            return response.Customer;
        }
        else
        {
            account.CustomerId = square.Id;

            var squareBirthday = string.IsNullOrEmpty(square.Birthday)
                ? (DateOnly?)null
                : DateOnly.Parse(square.Birthday);

            if ((square.GivenName ?? "") == (account.FirstName ?? "")
                && (square.FamilyName ?? "") == (account.LastName ?? "")
                && (square.EmailAddress ?? "") == (account.Email ?? "")
                && (square.PhoneNumber ?? "") == (phone ?? "")
                && squareBirthday == account.BirthDate)
                return square;

            var body = new UpdateCustomerRequest.Builder()
                .GivenName(account.FirstName)
                .FamilyName(account.LastName)
                .ReferenceId(account.AccountId)
                .EmailAddress(account.Email)
                .ReferenceId(account.AccountId);
            if (account.BirthDate.HasValue)
                body.Birthday(account.BirthDate.Value.ToString("yyyy-MM-dd"));
            else
                body.UnsetBirthday();

            if (hasPhone)
                body.PhoneNumber(phone);
            else
                body.UnsetPhoneNumber();

            _logger.LogInformation("Square: Updated Customer {Email}", account.Email);
            var response = await _client.CustomersApi.UpdateCustomerAsync(square.Id, body.Build());

            return response.Customer;
        }
    }

    private async Task UpdateTeamMember(SiteAccount account, TeamMember? square)
    {
        var teamMember = new TeamMember.Builder()
            .GivenName(account.FirstName)
            .FamilyName(account.LastName)
            .ReferenceId(account.AccountId)
            .EmailAddress(account.Email);

        if (SquareUtilities.TryFixPhoneNumber(account.Phone, out var phone))
            teamMember.PhoneNumber(phone);

        if (square == null)
        {
            var body = new CreateTeamMemberRequest(SquareUtilities.CreateIdempotencyKey(), teamMember.Build());
            await _client.TeamApi.CreateTeamMemberAsync(body);
            _logger.LogInformation("Square: Created Team Member {Email}", account.Email);
        }
        else
        {
            if ((square.GivenName ?? "") == (account.FirstName ?? "")
                && (square.FamilyName ?? "") == (account.LastName ?? "")
                && (square.EmailAddress ?? "") == (account.Email ?? "")
                && (square.PhoneNumber ?? "") == (phone ?? ""))
                return;

            var body = new UpdateTeamMemberRequest(teamMember.Build());
            await _client.TeamApi.UpdateTeamMemberAsync(square.Id, body);
            _logger.LogInformation("Square: Updated Team Member {Email}", account.Email);
        }
    }

    private async Task<WebhookSubscription[]> GetWebhookSubscriptions() =>
        await SquareUtilities.ListAsync(
            async () => await _client.WebhookSubscriptionsApi.ListWebhookSubscriptionsAsync(),
            async (cursor) => await _client.WebhookSubscriptionsApi.ListWebhookSubscriptionsAsync(cursor),
            response => response.Subscriptions,
            response => response.Cursor
        );

    private async Task<Location[]> GetLocations()
    {
        var locations = await _client.LocationsApi.ListLocationsAsync();
        return locations.Locations.ToArray();
    }

    private async Task<TeamMember[]> GetTeamMembers() =>
        await SquareUtilities.ListAsync(
            async () => await _client.TeamApi.SearchTeamMembersAsync(new SearchTeamMembersRequest()),
            async (cursor) =>
                await _client.TeamApi.SearchTeamMembersAsync(new SearchTeamMembersRequest(cursor: cursor)),
            response => response.TeamMembers,
            response => response.Cursor
        );

    private async Task<Customer[]> GetCustomers() =>
        await SquareUtilities.ListAsync(
            async () => await _client.CustomersApi.ListCustomersAsync(),
            async (cursor) => await _client.CustomersApi.ListCustomersAsync(cursor),
            response => response.Customers,
            response => response.Cursor
        );

    private async ValueTask<string> GetDefaultLocationId()
    {
        if (!string.IsNullOrEmpty(_locationId))
            return _locationId;

        var locations = await GetLocations();
        var location = locations.FirstOrDefault(location => location.Name == _options.Location.Name)
                       ?? await CreateDefaultLocation();
        _locationId = location.Id;
        return _locationId;
    }

    private async Task<Location> CreateDefaultLocation()
    {
        var location = new Location.Builder()
            .BusinessName(_options.Location.BusinessName)
            .Description("Used for online transactions by Mystwood Tavern website")
            .Country(_options.Location.Country)
            .Name(_options.Location.Name)
            .Status("ACTIVE")
            .Timezone(_options.Location.TimeZone)
            .Type("MOBILE")
            .WebsiteUrl(_options.Location.Url)
            .Build();
        var request = new CreateLocationRequest.Builder()
            .Location(location)
            .Build();
        var result = await _client.LocationsApi.CreateLocationAsync(request);
        _logger.LogInformation("Square: Created Default Location");
        return result.Location;
    }
}