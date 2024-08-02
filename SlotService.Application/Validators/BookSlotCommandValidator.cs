using FluentResults;
using SlotService.Application.API;
using SlotService.Application.API.Errors;


namespace SlotService.Application.Validators;

public class BookSlotCommandValidator : IValidator<BookSlotCommand>
{
    public Result Validate(BookSlotCommand value)
    {
        var result = new Result();

        if (value.Date < DateOnly.FromDateTime(DateTime.Today))
        {
            result.WithError(new SlotInThePastError(value.Date));
        }
        if (value.Start > value.End)
        {
            result.WithError(new StartAfterEndError(value.Start, value.End));
        }

        return result;
    }
}
