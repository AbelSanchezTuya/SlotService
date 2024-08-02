namespace SlotService.API.REST.Model;

/// <summary>
///     Containing the current schedule information for a given day.
/// </summary>
public class DaySchedule
{
    public required WorkPeriod WorkPeriod { get; set; }
    /// <summary>
    ///     Contains the slots that are already taken.
    /// </summary>
    public List<Slot> BusySlots { get; set; } = [];
}
