using FluentResults;


namespace SlotService.Application.API.Errors;

public class WeekNotAvailableError(DateOnly date)
    : Error($"Week containing date {date} is not available") { }
