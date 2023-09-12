namespace Larp.Square;

public class SquareOptions
{
    public const string SectionName = "Square";

    public string? ApplicationId { get; set; }
    public string? AccessToken { get; set; }
    public string? SquareUrl { get; set; }
    public string? CallbackUrl { get; set; }
    public string? SignatureKey { get; set; }
    public string? LocationId { get; set; }
    public string? ReturnUrl { get; set; }
}