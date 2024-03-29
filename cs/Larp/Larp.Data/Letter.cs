using System.Text.Json.Serialization;

namespace Larp.Data;

public enum LetterState
{
    NotStarted,
    Draft,
    Submitted,
    Approved,
    Locked // used internally for locked letters
}

public enum LetterFieldType
{
    Removed,
    Text,
    YesNo,
    Selection,
    MultipleSelection,
    Rating,
    TextBlock,
    Header,
    Component,
    Components,
    Character,
    Characters
}

public enum LetterFieldConditionOperator
{
    Always = 0,
    Equals,
    NotEquals,
    Contains,
    DoesNotContain,
    IsEmpty,
    IsNotEmpty
}

public class LetterFieldCondition
{
    public string? FieldName { get; set; }
    public string? Value { get; set; }
    public LetterFieldConditionOperator Operator { get; set; }
}

public class LetterField
{
    private const StringComparison Sc = StringComparison.InvariantCultureIgnoreCase;
    public string Name { get; set; } = default!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public LetterFieldType Type { get; set; }

    public List<string> Options { get; set; } = new();

    [BsonIgnoreIfNull] public LetterFieldCondition? Conditional { get; set; }

    public bool ShowField(Dictionary<string, string> fields)
    {
        var condition = Conditional;
        if (string.IsNullOrWhiteSpace(condition?.FieldName)) return true;
        if (condition.Operator == LetterFieldConditionOperator.Always)
            return true;

        var value = fields.TryGetValue(condition.FieldName, out var v) ? v : null;

        if (string.IsNullOrWhiteSpace(value))
            return condition.Operator is LetterFieldConditionOperator.IsEmpty;

        return condition.Operator switch
        {
            LetterFieldConditionOperator.IsNotEmpty => true,
            LetterFieldConditionOperator.Equals => string.Equals(value, condition.Value, Sc),
            LetterFieldConditionOperator.NotEquals => !string.Equals(value, condition.Value, Sc),
            LetterFieldConditionOperator.Contains => value.Contains(condition.Value ?? "---", Sc),
            LetterFieldConditionOperator.DoesNotContain => !value.Contains(condition.Value ?? "---", Sc),
            _ => false
        };
    }
}

public class EventsAndLetters
{
    public Dictionary<string, AccountName> Accounts { get; init; } = new();
    public Dictionary<string, Event> Events { get; init; } = new();
    public Dictionary<string, Letter> Letters { get; init; } = new();
    public Dictionary<string, LetterTemplate> LetterTemplates { get; init; } = new();
}

public class LettersAndTemplate
{
    public LetterTemplate? LetterTemplate { get; set; }
    public Letter[] Letters { get; set; } = Array.Empty<Letter>();
    public Event Event { get; set; } = default!;
}

public class LetterAndTemplate
{
    public string AccountId { get; set; } = default!;
    public string AccountName { get; set; } = default!;
    public LetterTemplate LetterTemplate { get; set; } = default!;
    public Letter Letter { get; set; } = default!;
    public Event? Event { get; set; }
}

public class LetterTemplate
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string LetterTemplateId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Title { get; set; } = default!;

    public LetterField[] Fields { get; set; } = Array.Empty<LetterField>();

    public string? Description { get; set; }

    public bool Retired { get; set; }
}

public class Letter
{
    [BsonId, BsonRepresentation(BsonType.ObjectId), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string LetterId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string TemplateId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string AccountId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string EventId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public LetterState State { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? StartedOn { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? SubmittedOn { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? ApprovedOn { get; set; }

    [BsonRepresentation(BsonType.ObjectId), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ApprovedBy { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, string> Fields { get; set; } = new();

    public ChangeLog[] ChangeLog { get; set; } = Array.Empty<ChangeLog>();
}