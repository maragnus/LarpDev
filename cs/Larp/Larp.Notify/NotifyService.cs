﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using Larp.Notify.Sendinblue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Larp.Notify;

public class NotifyServiceOptions
{
    public const string SectionName = "Notifications";

    public EmailOptions Email { get; set; }
}

public class EmailOptions
{
    public List<EmailAddress> Senders { get; set; } = new();
    public string ApiKey { get; set; } = null!;
    public string ApiEndPoint { get; set; } = null!;
}

public interface INotifyService
{
    public Task SendEmailAsync(string recipient, string subject, string body);
}

public class NotifyService : INotifyService
{
    private readonly HttpClient _client;
    private readonly ILogger<NotifyService> _logger;
    private readonly EmailOptions _options;

    public NotifyService(IOptions<EmailOptions> options, ILogger<NotifyService> logger)
    {
        _logger = logger;
        _options = options.Value;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        _client.DefaultRequestHeaders.Add("api-key", _options.ApiKey);
    }

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var sender = _options.Senders.First();
        var to = EmailAddress.Parse(recipient);
        var email = new Email(sender, to, subject, body);

        try
        {
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