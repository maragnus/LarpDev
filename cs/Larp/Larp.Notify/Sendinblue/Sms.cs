using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Larp.Notify.Sendinblue;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public record Sms(
    [property: JsonPropertyName("sender")] string Sender,
    [property: JsonPropertyName("recipient")] string To,
    [property: JsonPropertyName("content")] string? Content,
    [property: JsonPropertyName("unicodeEnabled")] bool UnicodeEnabled,
    [property: JsonPropertyName("organisationPrefix")] string OrganisationPrefix,
    [property: JsonPropertyName("webUrl")] string CallbackUrl = "",
        [property: JsonPropertyName("type")] string Type = "transactional"
);