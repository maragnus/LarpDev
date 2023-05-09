using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Larp.Notify.Sendinblue;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public record Email(
    [property: JsonPropertyName("sender")] EmailAddress Sender,
    [property: JsonPropertyName("to")] List<EmailAddress> To,
    [property: JsonPropertyName("subject")] string? Subject,
    [property: JsonPropertyName("htmlContent")] string? HtmlBody
);