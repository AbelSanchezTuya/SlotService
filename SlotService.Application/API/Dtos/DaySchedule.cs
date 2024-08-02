namespace SlotService.Application.API.Dtos;

public class DaySchedule
{
    public List<Slot> BusySlots = [];
    public WorkPeriod WorkPeriod = new();
    public DayOfWeek Name { get; set; }
}
