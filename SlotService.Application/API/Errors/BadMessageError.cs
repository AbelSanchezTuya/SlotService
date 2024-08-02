using FluentResults;


namespace SlotService.Application.API.Errors;

public class BadMessageError(string handler, string incomingMessage, string expectedMessage)
    : Error(
        $"Try to handle message {incomingMessage} with {handler} when should be {expectedMessage}") { }
