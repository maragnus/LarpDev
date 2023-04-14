using Larp.Data;
using Larp.Data.MwFifth;

namespace Larp.Landing.Shared;

public class PlayerAttendee
{
    public string? AccountId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Age { get; set; }
    public CharacterAttendee[] Characters { get; set; } = Array.Empty<CharacterAttendee>();
    public string? Notes { get; set; }
    public Letter? PreEventLetter { get; set; }
    public int? DiscountPercentage { get; set; }
}

public class CharacterAttendee
{
    public string? CharacterId { get; set; }
    public string? RevisionId { get; set; }
    public string Name { get; set; } = default!;
    public string HomeChapter { get; set; } = default!;
    public string? Notes { get; set; }
    public string? GeneratedNotes { get; set; }
    public NameRank[] Skills { get; set; } = Array.Empty<NameRank>();
    public NameRank[] Advantages { get; set; } = Array.Empty<NameRank>();
    public NameRank[] Disadvantages { get; set; } = Array.Empty<NameRank>();
}

public class PreregistrationNotes
{
    public Event Event { get; set; } = default!;
    public PlayerAttendee[] Attendees { get; set; } = Array.Empty<PlayerAttendee>();
}