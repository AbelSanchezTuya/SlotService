using FluentResults;
using SlotService.Application.API;
using SlotService.Application.API.Errors;
using SlotService.Application.Validators;
using SlotService.Domain;


namespace SlotService.Application.Handlers;

public class BookSlotCommandHandler(
    IValidator<BookSlotCommand> validator,
    IAgendaRepository repository)
    : BaseHandler<BookSlotCommand>
{
    protected override IResultBase Handle(BookSlotCommand command)
    {
        var result = validator.Validate(command);
        if (result.IsFailed)
        {
            return result;
        }

        var slotToBook = new Slot(command.Date, command.Start, command.End);
        if (!repository.TryToGetWeekSchedule(command.Date, out var weekSchedule) ||
            weekSchedule == null)
        {
            return Result.Fail(new WeekNotAvailableError(slotToBook.Date));
        }
        if (weekSchedule.CanAccomodate(slotToBook))
        {
            weekSchedule.Book(slotToBook);
            repository.UpdateWeekSchedule(weekSchedule);

            return Result.Ok();
        }

        return Result.Fail(new NotAvailableSlotError(slotToBook));
    }
}
