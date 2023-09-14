using System.ComponentModel.DataAnnotations;

namespace Larp.Payments;

public class SquareOptions
{
    public const string SectionName = "Square";

    public string? ApplicationId { get; set; }
    public string? AccessToken { get; set; }
    public string? Environment { get; set; }
    public string? ReturnUrl { get; set; }
    public string? DepositItemName { get; set; }
    public SquareWebhookOptions Webhook { get; set; } = new();
    public SquareLocationOptions Location { get; set; } = new();

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApplicationId))
            throw new ValidationException($"{nameof(ApplicationId)} is required");
        if (string.IsNullOrWhiteSpace(AccessToken)) throw new ValidationException($"{nameof(AccessToken)} is required");
        if (Environment != "Sandbox" && Environment != "Production")
            throw new ValidationException($"{nameof(Environment)} must be either Production or Sandbox");
        if (string.IsNullOrWhiteSpace(ReturnUrl)) throw new ValidationException($"{nameof(ReturnUrl)} is required");
        if (string.IsNullOrWhiteSpace(DepositItemName))
            throw new ValidationException($"{nameof(DepositItemName)} is required");
        Webhook.Validate();
        Location.Validate();
    }
}

public class SquareLocationOptions
{
    public string? BusinessName { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string TimeZone { get; set; } = "America/New_York";
    public string Country { get; set; } = "US";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BusinessName))
            throw new ValidationException($"{nameof(BusinessName)} is required");
        if (string.IsNullOrWhiteSpace(Name)) throw new ValidationException($"{nameof(Name)} is required");
        if (string.IsNullOrWhiteSpace(Url)) throw new ValidationException($"{nameof(Url)} is required");
    }
}

public class SquareWebhookOptions
{
    public string? Name { get; set; }
    public string? CallbackUrl { get; set; }
    public string[] AllowedIps { get; set; } = Array.Empty<string>();

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ValidationException($"{nameof(Name)} is required");
        if (string.IsNullOrWhiteSpace(CallbackUrl)) throw new ValidationException($"{nameof(CallbackUrl)} is required");
    }
}