namespace SlotService.Application.API.Dtos;

public class Slot
{
    public DateOnly Date { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}
