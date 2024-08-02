using SlotService.Application.Handlers;
using SlotService.Application.Mappers;
using SlotService.Application.Validators;
using SlotService.Storage;


namespace SlotService.Application.API.Common;

public static class MessageDispatcherFactory
{
    public static IMessageDispatcher Create()
    {
        var getWeeklyAvailabilityQueryValidator = new GetWeeklyAvailabilityQueryValidator();
        var getWeeklyAvailabilityResponseMapper = new GetWeekAvailabilityResponseMapper();
        var bookSlotCommandValidator = new BookSlotCommandValidator();
        var agendaRepo = InMemoryAgendaRepository.Instance;
        var getWeeklyAvailabilityQueryHandler = new GetWeeklyAvailabilityQueryHandler(
            getWeeklyAvailabilityQueryValidator,
            getWeeklyAvailabilityResponseMapper,
            agendaRepo);
        var bookSlotCommandHandler = new BookSlotCommandHandler(
            bookSlotCommandValidator,
            agendaRepo);

        var messageDispatcher = new MessageDispatcher();
        messageDispatcher.RegisterHandler<BookSlotCommand>(bookSlotCommandHandler)
                         .RegisterHandler<GetWeeklyAvailabilityQuery>(
                              getWeeklyAvailabilityQueryHandler);

        return messageDispatcher;
    }
}
