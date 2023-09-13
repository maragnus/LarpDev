using System.ComponentModel.DataAnnotations;

namespace Larp.Payments;

public class SquareOptions
{
    public const string SectionName = "Square";

    public string? ApplicationId { get; set; }
    public string? AccessToken { get; set; }
    public string? SignatureKey { get; set; }
    public string? LocationId { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Environment { get; set; } = "Sandbox";
    public string? CallbackUrl { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApplicationId))
            throw new ValidationException($"{nameof(ApplicationId)} is required");
        if (string.IsNullOrWhiteSpace(AccessToken)) throw new ValidationException($"{nameof(AccessToken)} is required");
        if (string.IsNullOrWhiteSpace(SignatureKey))
            throw new ValidationException($"{nameof(SignatureKey)} is required");
        if (string.IsNullOrWhiteSpace(LocationId)) throw new ValidationException($"{nameof(LocationId)} is required");
        if (string.IsNullOrWhiteSpace(ReturnUrl)) throw new ValidationException($"{nameof(ReturnUrl)} is required");
        if (string.IsNullOrWhiteSpace(CallbackUrl)) throw new ValidationException($"{nameof(CallbackUrl)} is required");
        if (Environment != "Sandbox" && Environment != "Production")
            throw new ValidationException($"{nameof(Environment)} must be either Production or Sandbox");
    }
}