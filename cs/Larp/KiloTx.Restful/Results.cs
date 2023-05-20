using System.Text.Json.Serialization;

namespace KiloTx.Restful;

public record StringResult(
    bool IsSuccess,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? Value,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? ErrorMessage)
{
    public static StringResult Success(string value) => new(true, value, null);
    public static StringResult Failed(string errorMessage) => new(false, null, errorMessage);
}

public record Result(
    bool IsSuccess,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? ErrorMessage)
{
    public static Result Success { get; } = new(true, null);
    public static Result Failed(string errorMessage) => new(false, errorMessage);
}