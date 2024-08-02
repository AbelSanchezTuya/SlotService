namespace SlotService.Application.API.Dtos;

public class WorkPeriod
{
    public TimeOnly StartHour { get; set; }
    public TimeOnly LunchStartHour { get; set; }
    public TimeOnly LunchEndHour { get; set; }
    public TimeOnly EndHour { get; set; }
}
