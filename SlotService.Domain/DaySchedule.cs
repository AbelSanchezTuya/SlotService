namespace SlotService.Domain;

public class DaySchedule
{
    public DayOfWeek DayOfWeek { get; init; }
    public required WorkPeriod WorkPeriod { get; init; }
    public List<Slot> BusySlots { get; } = new(1000);
}
