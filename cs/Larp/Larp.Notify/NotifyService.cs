using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using KiloTx.Restful;
using Larp.Common;
using Larp.Notify.Sendinblue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Larp.Notify;

public class NotifyServiceOptions
{
    public const string SectionName = "Notifications";

    public string SiteUrl { get; set; } = "https://localhost";
    public string ApiKey { get; set; } = default!;
    public EmailOptions Email { get; set; } = new();
    public SmsOptions Sms { get; set; } = new();
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class EmailOptions
{
    public List<string> Senders { get; set; } = new();
    public string ApiEndPoint { get; set; } = default!;
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class SmsOptions
{
    public string ApiEndPoint { get; set; } = default!;
    public string Sender { get; set; } = default!;
    public string OrganisationPrefix { get; set; } = default!;
}

public class NotifyService : INotifyService
{
    private readonly HttpClient _client;
    private readonly ILogger<NotifyService> _logger;
    private readonly NotifyServiceOptions _options;

    public NotifyService(IOptions<NotifyServiceOptions> options, ILogger<NotifyService> logger)
    {
        _logger = logger;
        _options = options.Value;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        _client.DefaultRequestHeaders.Add("api-key", _options.ApiKey);
        SiteUrl = options.Value.SiteUrl;
    }

    public string SiteUrl { get; }

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var sender = EmailAddress.Parse(_options.Email.Senders.First());
        var to = EmailAddress.Parse(recipient);

        if (string.IsNullOrWhiteSpace(to.DisplayName))
            to = to with { DisplayName = null };

        var email = new Email(sender, new() { to }, subject, body);

        try
        {
            var data = JsonSerializer.SerializeToDocument(email);

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogWarning("Cannot send email because ApiKey is empty: {Email}", data.RootElement.ToString());
                return;
            }
            
            _logger.LogInformation("Sending Email: {Email}", data.RootElement.ToString());

            var result = await _client.PostAsJsonAsync(_options.Email.ApiEndPoint, email);
            if (!result.IsSuccessStatusCode)
            {
                var reply = await result.Content.ReadAsStringAsync();
                _logger.LogWarning("Status Code {StatusCode} {StatusCodeName}: {JsonResponse}", (int)result.StatusCode,
                    result.StatusCode, reply);
                throw new NotifyException("Email failed to send due to service exception");
            }
        }
        catch (Exception ex)
        {
            throw new NotifyException("Email failed to send due to exception", ex);
        }
    }

    public ValueTask<bool> IsSmsRecipientValid(string recipient)
    {
        var number = Regex.Replace(recipient, @"[^\d]", "");
        return ValueTask.FromResult(number.Length == 10 || (number.Length == 11 && number.StartsWith("1")));
    }

    public async Task SendSmsAsync(string recipient, string body)
    {
        if (!await IsSmsRecipientValid(recipient))
            throw new BadRequestException($"Recipient \"{recipient}\" is not value");

        var number = Regex.Replace(recipient, @"[^\d]", "");
        if (number.Length == 10)
            number = "1" + number;
        
        var sms = new Sms(_options.Sms.Sender, number, body, true, _options.Sms.OrganisationPrefix);
        
        try
        {
            var data = JsonSerializer.SerializeToDocument(sms);

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogWarning("Cannot send Sms because ApiKey is empty: {Sms}", data.RootElement.ToString());
                return;
            }
            
            _logger.LogInformation("Sending Sms: {Sms}", data.RootElement.ToString());

            var result = await _client.PostAsJsonAsync(_options.Sms.ApiEndPoint, sms);
            if (!result.IsSuccessStatusCode)
            {
                var reply = await result.Content.ReadAsStringAsync();
                _logger.LogWarning("Status Code {StatusCode} {StatusCodeName}: {JsonResponse}", (int)result.StatusCode,
                    result.StatusCode, reply);
                throw new NotifyException("Sms failed to send due to service exception");
            }
        }
        catch (Exception ex)
        {
            throw new NotifyException("Sms failed to send due to exception", ex);
        }
    }
}