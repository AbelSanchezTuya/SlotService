using FluentResults;


namespace SlotService.Application.API.Errors;

public class DispatchError(string messageName)
    : Error($"Cannot dispatch message {messageName}") { }
