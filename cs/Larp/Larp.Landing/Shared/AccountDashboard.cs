namespace Larp.Landing.Shared;

public class AccountDashboard
{
    public int? TotalMoonstone { get; init; }
    public int? AvailableMoonstone { get; init; }
    public Dictionary<string, CharacterSummary> Characters { get; init; } = new();
    public EventAttendanceList Events { get; init; } = new();
}