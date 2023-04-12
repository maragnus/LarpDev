namespace Larp.Common;

public record StringResult(bool IsSuccess, string? Value, string? ErrorMessage)
{
    public static StringResult Success(string value) => new StringResult(true, value, null);
    public static StringResult Failed(string errorMessage) => new StringResult(false, null, errorMessage);
}

public record Result(bool IsSuccess, string? ErrorMessage)
{
    public static Result Success { get; } = new Result(true, null);
    public static Result Failed(string errorMessage) => new Result(false, errorMessage);
}
