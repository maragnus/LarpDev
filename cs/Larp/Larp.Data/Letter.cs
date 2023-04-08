namespace Larp.Data;

public enum LetterState
{
    Draft,
    Submitted,
    Approved
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
    Header
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
    public string Name { get; set; } = default!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public LetterFieldType Type { get; set; }

    public List<string> Options { get; set; } = new();

    [BsonIgnoreIfNull] public LetterFieldCondition? Conditional { get; set; }

    private const StringComparison Sc = StringComparison.InvariantCultureIgnoreCase;

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

public class LettersAndTemplate
{
    public LetterTemplate? LetterTemplate { get; set; } = default!;
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
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string LetterId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string TemplateId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = default!;

    public LetterState State { get; set; }

    public DateTimeOffset? StartedOn { get; set; }
    public DateTimeOffset? SubmittedOn { get; set; }
    public DateTimeOffset? ApprovedOn { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ApprovedBy { get; set; }

    public Dictionary<string, string> Fields { get; set; } = new();
}