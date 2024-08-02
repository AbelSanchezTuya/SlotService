using FluentResults;


namespace SlotService.Application.API.Errors;

public class SlotInThePastError(DateOnly date) : Error(
    $"Cannot book slot with date {date} in the past") { }
