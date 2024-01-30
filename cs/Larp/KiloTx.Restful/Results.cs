namespace KiloTx.Restful;

public record StringResult
{
    public StringResult(bool isSuccess,
        string? value,
        string? errorMessage)
    {
        this.IsSuccess = isSuccess;
        this.Value = value;
        this.ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public string? Value { get; }
    public string? ErrorMessage { get; }

    public static StringResult Success(string value) => new(true, value, null);
    public static StringResult Failed(string errorMessage) => new(false, null, errorMessage);

    public void Deconstruct(out bool IsSuccess, out string? Value, out string? ErrorMessage)
    {
        IsSuccess = this.IsSuccess;
        Value = this.Value;
        ErrorMessage = this.ErrorMessage;
    }
}

public record Result
{
    public Result(bool isSuccess,
        string? errorMessage)
    {
        this.IsSuccess = isSuccess;
        this.ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    public static Result Success { get; } = new(true, null);
    public static Result Failed(string errorMessage) => new(false, errorMessage);

    public void Deconstruct(out bool IsSuccess, out string? ErrorMessage)
    {
        IsSuccess = this.IsSuccess;
        ErrorMessage = this.ErrorMessage;
    }
}