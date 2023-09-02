namespace Larp.Landing.Shared;

public class AccountDashboard
{
    public int? TotalMoonstone { get; set; }
    public int? AvailableMoonstone { get; set; }
    public Dictionary<string, CharacterSummary> Characters { get; set; } = new();
    public Dictionary<string, Event> Events { get; set; } = new();
    public Dictionary<string, Letter> Letters { get; set; } = new();
}