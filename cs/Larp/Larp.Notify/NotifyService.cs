using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Larp.Common;
using Larp.Notify.Sendinblue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Larp.Notify;

public class NotifyServiceOptions
{
    public const string SectionName = "Notifications";

    public string SiteUrl { get; set; } = "https://localhost";
    public EmailOptions Email { get; set; } = new EmailOptions();
}

public class EmailOptions
{
    public List<string> Senders { get; set; } = new();
    public string ApiKey { get; set; } = default!;
    public string ApiEndPoint { get; set; } = default!;
}

public class NotifyService : INotifyService
{
    private readonly HttpClient _client;
    private readonly ILogger<NotifyService> _logger;
    private readonly EmailOptions _options;

    public NotifyService(IOptions<NotifyServiceOptions> options, ILogger<NotifyService> logger)
    {
        _logger = logger;
        _options = options.Value.Email;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        _client.DefaultRequestHeaders.Add("api-key", _options.ApiKey);
        SiteUrl = options.Value.SiteUrl;
    }

    public string SiteUrl { get; }

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var sender = EmailAddress.Parse(_options.Senders.First());
        var to = EmailAddress.Parse(recipient);

        if (string.IsNullOrWhiteSpace(to.DisplayName))
            to = to with { DisplayName = null };
        
        var email = new Email(sender, new() { to }, subject, body);

        try
        {
            var data = JsonSerializer.SerializeToDocument(email);
            _logger.LogWarning(data.RootElement.ToString());
            
            var result = await _client.PostAsJsonAsync(_options.ApiEndPoint, email);
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
}