using System.Net.Mail;
using System.Text.Json.Serialization;

namespace Larp.Notify.Sendinblue;

public record EmailAddress(
    [property: JsonPropertyName("email")] string Address,
    [property: JsonPropertyName("name")] string? DisplayName)
{
    public static EmailAddress Parse(string value)
    {
        if (!MailAddress.TryCreate(value, out var email))
            throw new ArgumentException("Invalid email address", nameof(value));
        return new EmailAddress(email.Address, email.DisplayName);
    }
}