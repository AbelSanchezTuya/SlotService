using FluentResults;
using SlotService.Application.API;
using SlotService.Application.API.Errors;
using SlotService.Application.Mappers;
using SlotService.Application.Validators;
using SlotService.Domain;


namespace SlotService.Application.Handlers;

public class GetWeeklyAvailabilityQueryHandler(
    IValidator<GetWeeklyAvailabilityQuery> validator,
    IMapper<GetWeekAvailabilityResponse, IWeekSchedule> mapper,
    IAgendaRepository repository)
    : BaseHandler<GetWeeklyAvailabilityQuery>
{
    protected override IResultBase Handle(GetWeeklyAvailabilityQuery request)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        if (!repository.TryToGetWeekSchedule(request.Date, out var weekSchedule) ||
            weekSchedule == null)
        {
            return Result.Fail(new WeekNotAvailableError(request.Date));
        }

        var response = mapper.Map(weekSchedule);

        return Result.Ok(response);
    }
}
