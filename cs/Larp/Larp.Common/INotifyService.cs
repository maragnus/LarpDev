namespace Larp.Common;

public interface INotifyService
{
    public string SiteUrl { get; }
    public Task SendEmailAsync(string recipient, string subject, string body);
}