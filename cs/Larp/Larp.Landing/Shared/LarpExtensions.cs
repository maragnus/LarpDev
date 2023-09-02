namespace Larp.Landing.Shared;

public static class LarpExtensions
{
    public static bool IsPreEventOpen(this Event ev) =>
        ev.LetterTemplates.Any(lt => lt is { IsOpen: true, Name: LetterNames.PreEvent });

    public static bool IsPostEventOpen(this Event ev) =>
        ev.LetterTemplates.Any(lt => lt is { IsOpen: true, Name: LetterNames.PostEvent });
}