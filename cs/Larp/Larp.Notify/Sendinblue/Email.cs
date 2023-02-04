using System.Text.Json.Serialization;

namespace Larp.Notify.Sendinblue;

public record Email(
    [property: JsonPropertyName("sender")] EmailAddress Sender,
    [property: JsonPropertyName("to")] List<EmailAddress> To,
    [property: JsonPropertyName("subject")]
    string? Subject,
    [property: JsonPropertyName("htmlContent")]
    string? HtmlBody
)
{
}