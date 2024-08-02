using SlotService.Application.API.Common;
using SlotService.Application.API.Dtos;


namespace SlotService.Application.API;

public class BookSlotCommand : IMessage
{
    public DateOnly Date { get; init; }
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
    public string Comments { get; init; } = string.Empty;
    public required Patient Patient { get; init; }
}
