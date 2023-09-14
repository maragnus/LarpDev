using System.Text.RegularExpressions;
using Larp.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Square;
using Square.Exceptions;
using Square.Models;
using Environment = Square.Environment;

namespace Larp.Payments;

public interface ISquareService
{
    Task<SquarePaymentUrl> CreatePaymentUrl(string transactionId, decimal amount, string itemName, SiteAccount account);

    Task<Payment[]> GetPayments();

    Task Synchronize(SiteAccount[] accounts);
}

public partial class SquareService : ISquareService
{
    private static string? _locationId;
    private readonly SquareClient _client;
    private readonly ILogger<SquareService> _logger;
    private readonly SquareOptions _options;

    public SquareService(IOptions<SquareOptions> options, ILogger<SquareService> logger)
    {
        _logger = logger;
        _options = options.Value;
        _options.Validate();
        _client = CreateClient();
    }

    public static string? SignatureKey { get; private set; }

    public async Task Synchronize(SiteAccount[] accounts)
    {
        try
        {
            _logger.LogInformation("Square: Updating Webhook Subscription");
            await UpdateWebhookSubscription();

            _logger.LogInformation("Square: Updating Default Location");
            await GetDefaultLocationId();

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

    public async Task<Payment[]> GetPayments() =>
        await ListAsync(
            async () => await _client.PaymentsApi.ListPaymentsAsync(),
            async (cursor) => await _client.PaymentsApi.ListPaymentsAsync(cursor),
            response => response.Payments,
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

            var body = new CreatePaymentLinkRequest.Builder()
                .IdempotencyKey(CreateIdempotencyKey())
                .PaymentNote($"{customer.GivenName} {customer.FamilyName} {customer.EmailAddress}".Trim())
                .Order(new Order.Builder(locationId)
                    .CustomerId(customer.Id)
                    .ReferenceId(transactionId)
                    .LineItems(new[]
                    {
                        new OrderLineItem.Builder("1")
                            .Name(itemName)
                            .BasePriceMoney(money)
                            .Build()
                    })
                    .Build())
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

    private SquareClient CreateClient() =>
        new SquareClient.Builder()
            .AccessToken(_options.AccessToken)
            .SquareVersion("2023-08-16")
            .Environment(
                Enum.TryParse<Square.Environment>(_options.Environment, out var env)
                    ? env
                    : Environment.Sandbox)
            .Build();

    private async Task UpdateWebhookSubscription()
    {
        var subscriptions = await GetWebhookSubscriptions();
        var subscription =
            subscriptions.FirstOrDefault(subscription => subscription.NotificationUrl == _options.Webhook.CallbackUrl);
        if (subscription is null)
        {
            var response = await _client.WebhookSubscriptionsApi.CreateWebhookSubscriptionAsync(
                new CreateWebhookSubscriptionRequest.Builder(new WebhookSubscription.Builder()
                        .Name(_options.Webhook.Name)
                        .NotificationUrl(_options.Webhook.CallbackUrl)
                        .Enabled(true)
                        .EventTypes(new[] { "payment.created", "payment.updated" })
                        .Build())
                    .IdempotencyKey(CreateIdempotencyKey())
                    .Build());
            SignatureKey = response.Subscription.SignatureKey;
            _logger.LogInformation("Square: Created Webhook Subscription");
            return;
        }

        var key = await _client.WebhookSubscriptionsApi
            .RetrieveWebhookSubscriptionAsync(subscription.Id);
        SignatureKey = key.Subscription.SignatureKey;
        _logger.LogInformation("Square: Received Webhook Subscription");
    }

    private async Task SynchronizeTeamMembers(SiteAccount[] siteAccounts)
    {
        var teamMembers = await GetTeamMembers();
        var accounts = teamMembers
            .Join(siteAccounts, member => member.ReferenceId, account => account.AccountId, (member, account) => new
            {
                Square = (TeamMember?)member,
                Account = account
            })
            .Concat(
                siteAccounts
                    .Where(account => teamMembers.All(member => member.ReferenceId != account.AccountId))
                    .Select(account => new
                    {
                        Square = (TeamMember?)null,
                        Account = account
                    })
            )
            .ToList();

        foreach (var item in accounts)
        {
            var account = item.Account;
            var square = item.Square;

            if (account.FinancialAccess)
                await UpdateTeamMember(account, square);
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
            var account = item.Account;
            var square = item.Square;

            await UpdateCustomer(account, square);
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
        var hasPhone = TryFixPhoneNumber(account.Phone, out var phone);

        if (square == null)
        {
            var body = new CreateCustomerRequest.Builder()
                .IdempotencyKey(CreateIdempotencyKey())
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

            return response.Customer;
        }
        else
        {
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

        if (TryFixPhoneNumber(account.Phone, out var phone))
            teamMember.PhoneNumber(phone);

        if (square == null)
        {
            var body = new CreateTeamMemberRequest(CreateIdempotencyKey(), teamMember.Build());
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

    private static bool TryFixPhoneNumber(string? phone, out string sanitizedPhone)
    {
        sanitizedPhone = "";

        if (string.IsNullOrWhiteSpace(phone))
            return false;

        sanitizedPhone = RegexToNumeric().Replace(phone.Trim(), "");

        if (phone.StartsWith('+') && sanitizedPhone.Length > 5)
        {
            sanitizedPhone = $"+1{sanitizedPhone}";
            return true;
        }

        if (sanitizedPhone.Length == 10)
        {
            sanitizedPhone = $"+1{sanitizedPhone}";
            return true;
        }

        if (sanitizedPhone.Length == 11 && sanitizedPhone.StartsWith("1"))
        {
            sanitizedPhone = $"+{sanitizedPhone}";
            return true;
        }

        return false;
    }

    [GeneratedRegex("[^\\d]")]
    private static partial Regex RegexToNumeric();

    private static string CreateIdempotencyKey() => Guid.NewGuid().ToString();

    private static async Task<TItem[]> ListAsync<TItem, TResponse>(
        Func<Task<TResponse>> firstRequest,
        Func<string, Task<TResponse>> nextRequest,
        Func<TResponse, IList<TItem>?> getItems,
        Func<TResponse, string?> getCursor)
    {
        var items = new List<TItem>();
        var response = await firstRequest();
        var responseItems = getItems(response);
        while (responseItems is not null)
        {
            var count = items.Count;
            items.AddRange(responseItems);

            var cursor = getCursor(response);
            if (items.Count == count || string.IsNullOrEmpty(cursor)) break;
            response = await nextRequest(cursor);
            responseItems = getItems(response)?.ToList();
        }

        return items.ToArray();
    }

    private async Task<WebhookSubscription[]> GetWebhookSubscriptions() =>
        await ListAsync(
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
        await ListAsync(
            async () => await _client.TeamApi.SearchTeamMembersAsync(new SearchTeamMembersRequest()),
            async (cursor) =>
                await _client.TeamApi.SearchTeamMembersAsync(new SearchTeamMembersRequest(cursor: cursor)),
            response => response.TeamMembers,
            response => response.Cursor
        );

    private async Task<Customer[]> GetCustomers() =>
        await ListAsync(
            async () => await _client.CustomersApi.ListCustomersAsync(),
            async (cursor) => await _client.CustomersApi.ListCustomersAsync(cursor),
            response => response.Customers,
            response => response.Cursor
        );

    private async ValueTask<string> GetDefaultLocationId()
    {
        if (!string.IsNullOrEmpty(_locationId))
            return _locationId;

        var location = await GetDefaultLocation();
        _locationId = location.Id;
        return _locationId;
    }

    private async ValueTask<Location> GetDefaultLocation()
    {
        var locations = await GetLocations();
        return locations
                   .FirstOrDefault(location => location.Name == _options.Location.Name)
               ?? await CreateDefaultLocation();
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

public static class SquareWebhook
{
    public static IServiceCollection AddSquareService(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<SquareCallbackService>()
            .AddScoped<ISquareService, SquareService>();

    public static async Task HandleCallbackAsync(HttpContext httpContext, SquareCallbackService callbackService)
    {
        await callbackService.HandleCallbackAsync(httpContext);
    }
}