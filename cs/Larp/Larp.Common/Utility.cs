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
}