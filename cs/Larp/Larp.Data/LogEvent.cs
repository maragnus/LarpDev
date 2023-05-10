using System.Text.Json.Serialization;

namespace Larp.Data;

public abstract class LogEvent
{
    protected LogEvent(string type) => Type = type;

    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string LogEventId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ActorAccountId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ActorSessionId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset ActedOn { get; set; }

    public string Type { get; set; }
}

public class AccountStateLogEvent : LogEvent
{
    public AccountStateLogEvent() : base("Account State")
    {
    }

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    public AccountState AccountState { get; set; }
 
    [BsonRepresentation(BsonType.String)]
    public string AccountStateName => AccountState.ToString();
}


public class AccountRoleLogEvent : LogEvent
{
    public AccountRoleLogEvent() : base("Account Role")
    {
    }

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;

    [BsonIgnoreIfNull, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AccountRole? AddRole { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public string? AddRoleName => AddRole?.ToString();

    
    [BsonIgnoreIfNull, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AccountRole? RemoveRole { get; set; }

    [BsonRepresentation(BsonType.String)]
    public string? RemoveRoleName => RemoveRole?.ToString();
}

public class AccountMergeLogEvent : LogEvent
{
    public AccountMergeLogEvent() : base("Account Merge")
    {
    }

    [BsonRepresentation(BsonType.ObjectId)]
    public string FromAccountId { get; set; } = default!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ToAccountId { get; set; } = default!;
}

public class AddAdminLogEvent : LogEvent
{
    public AddAdminLogEvent() : base("Add Admin")
    {
    }

    public string? FullName { get; set; }
    
    public string? EmailAddress { get; set; }
    
}


public class EventChangeLogEvent : LogEvent
{
    public EventChangeLogEvent() : base("Event Changed")
    {
    }

    [BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = default!;
    
    public string? Summary { get; set; }
    public string? ChangeSummary { get; set; }
}

public class GameStateLogEvent : LogEvent
{
    public GameStateLogEvent() : base("Game State Changed")
    {
    }

    public string GameName { get; set; } = default!;
    
    public string? Summary { get; set; }
}