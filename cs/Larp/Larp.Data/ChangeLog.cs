using System.Text.Json.Serialization;

namespace Larp.Data;

public class ChangeLog
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Action { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? State { get; set; }

    public DateTime ChangedOn { get; set; }

    [BsonRepresentation(BsonType.ObjectId), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ChangedBy { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Summary { get; set; }

    public static ChangeLog Log<TEnum>(string action, TEnum state, string accountId, string? changeSummary = null)
        where TEnum : Enum =>
        new()
        {
            Action = action,
            State = state.ToString(),
            ChangedOn = DateTime.UtcNow,
            ChangedBy = accountId,
            Summary = changeSummary
        };
}