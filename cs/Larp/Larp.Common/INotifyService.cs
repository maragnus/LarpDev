namespace Larp.Common;

public interface INotifyService
{
    public string SiteUrl { get; }
    public Task SendEmailAsync(string recipient, string subject, string body);
    public ValueTask<bool> IsSmsRecipientValid(string recipient);
    public Task SendSmsAsync(string recipient, string body);
}