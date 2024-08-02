using FluentResults;


namespace SlotService.Application.API.Errors;

public class StartAfterEndError(TimeOnly start, TimeOnly end) : Error(
    $"Start({start}) cannot be after end({end})") { }
