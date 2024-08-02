using SlotService.Application.API.Common;


namespace SlotService.Application.API;

public class GetWeeklyAvailabilityQuery : IMessage
{
    public DateOnly Date { get; set; }
}
