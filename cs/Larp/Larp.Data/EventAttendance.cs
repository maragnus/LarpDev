namespace Larp.Data;

[PublicAPI]
public class EventAttendance : Attendance
{
    public EventAttendance()
    {
    }

    public EventAttendance(Attendance attendance, Event @event, Letter[] letters)
    {
        Id = attendance.Id;
        AccountId = attendance.AccountId;
        MwFifth = attendance.MwFifth;
        EventId = attendance.EventId;
        Event = @event;
        Letters = letters;
    }

    public Event Event { get; set; } = default!;
    public Letter[] Letters { get; set; } = Array.Empty<Letter>();
}