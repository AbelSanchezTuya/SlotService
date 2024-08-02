using SlotService.Application.API.Dtos;


namespace SlotService.Application.API;

public class GetWeekAvailabilityResponse
{
    public required Facility Facility { get; set; }
    public int SlotDurationMinutes { get; set; }
    public List<DaySchedule> DaysSchedule { get; set; } = [];
}
