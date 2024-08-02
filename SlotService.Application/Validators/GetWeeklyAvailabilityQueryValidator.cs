using FluentResults;
using SlotService.Application.API;
using SlotService.Application.API.Errors;


namespace SlotService.Application.Validators;

public class GetWeeklyAvailabilityQueryValidator : IValidator<GetWeeklyAvailabilityQuery>
{
    public Result Validate(GetWeeklyAvailabilityQuery value)
    {
        if (!IsAfterBeginningOfCurrentWeek(value.Date))
        {
            return Result.Fail(new WeekInThePastError(value.Date));
        }

        return Result.Ok();
    }

    private static bool IsAfterBeginningOfCurrentWeek(DateOnly value)
    {
        var firstWeekDay = DayOfWeek.Monday;
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var offset = today.DayOfWeek - firstWeekDay;
        if (offset < 0)
        {
            offset += 7;
        }

        var firstDayOfWeek = today.AddDays(-offset);

        return firstDayOfWeek <= value;
    }
}
