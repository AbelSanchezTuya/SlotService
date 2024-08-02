using FluentResults;


namespace SlotService.Application.API.Errors;

public class WeekInThePastError(DateOnly date)
    : Error($"Week containing date {date} is in the past.") { }
