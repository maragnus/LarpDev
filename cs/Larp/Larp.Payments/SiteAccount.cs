namespace Larp.Payments;

public class SiteAccount
{
    public string? AccountId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; } // Must be E.164 format
    public bool FinancialAccess { get; set; }
    public DateOnly? BirthDate { get; set; }

    public void SetFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName) || !fullName.Contains(' '))
        {
            FirstName = fullName?.Trim();
            LastName = null;
            return;
        }

        var parts = fullName.Split(' ', 2);
        FirstName = parts[0].Trim();
        LastName = parts[1].Trim();
    }

    public SiteAccount WithFillName(string? fullName)
    {
        SetFullName(fullName);
        return this;
    }
}