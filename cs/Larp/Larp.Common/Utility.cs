using System.Text.RegularExpressions;

namespace Larp.Common;

public static class Utility
{
    public static (string Name, int Rank) SplitNameAndRank(string name)
    {
        var match = Regex.Match(name, @"^(.*) (\d+)$", RegexOptions.Compiled);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value)) 
            : (name, 0);
    }
    
    
    public static int? GetAge(this DateOnly? birthDate)
    {
        if (!birthDate.HasValue) return null;
        
        var now = DateTime.Now;
        var age = now.Year - birthDate.Value.Year;

        if (now.Month < birthDate.Value.Month || (now.Month == birthDate.Value.Month && now.Day < birthDate.Value.Day))
            age--;

        return age;
    }
    
    public static int GetAge(this DateTime birthDate)
    {
        var now = DateTime.Now;
        var age = now.Year - birthDate.Year;

        if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
            age--;

        return age;
    }
}