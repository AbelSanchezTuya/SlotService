using SlotService.API.REST.Model;
using SlotService.Application.API;


namespace SlotService.API.REST.Mappers;

public class GetWeeklyAvailabilityResponseMapper
    : IMapper<WeekSchedule, GetWeekAvailabilityResponse>
{
    public WeekSchedule Map(GetWeekAvailabilityResponse input)
    {
        return new WeekSchedule
               {
                   Facility = ConvertFacility(input.Facility),
                   SlotDurationMinutes = input.SlotDurationMinutes,
                   Monday = ConvertDay(
                       input.DaysSchedule.FirstOrDefault(
                           x => x.Name == DayOfWeek.Monday)),
                   Tuesday = ConvertDay(
                       input.DaysSchedule.FirstOrDefault(
                           x => x.Name == DayOfWeek.Tuesday)),
                   Wednesday = ConvertDay(
                       input.DaysSchedule.FirstOrDefault(
                           x => x.Name == DayOfWeek.Wednesday)),
                   Thursday = ConvertDay(
                       input.DaysSchedule.FirstOrDefault(
                           x => x.Name == DayOfWeek.Thursday)),
                   Friday = ConvertDay(
                       input.DaysSchedule.FirstOrDefault(
                           x => x.Name == DayOfWeek.Friday))
               };
    }

    private DaySchedule? ConvertDay(Application.API.Dtos.DaySchedule? firstOrDefault)
    {
        if (firstOrDefault == null)
        {
            return null;
        }

        return new DaySchedule
               {
                   WorkPeriod = ConvertWorkPerido(firstOrDefault.WorkPeriod),
                   BusySlots = ConvertSlots(firstOrDefault.BusySlots).ToList()
               };
    }

    private IEnumerable<Slot> ConvertSlots(List<Application.API.Dtos.Slot> busySlots)
    {
        foreach (var slot in busySlots)
        {
            yield return new Slot
                         {
                             Start = slot.Date.ToDateTime(slot.Start).ToString(Slot.DateFormat),
                             End = slot.Date.ToDateTime(slot.End).ToString(Slot.DateFormat)
                         };
        }
    }

    private WorkPeriod ConvertWorkPerido(Application.API.Dtos.WorkPeriod workPeriod)
    {
        return new WorkPeriod
               {
                   StartHour = workPeriod.StartHour.Hour,
                   LunchStartHour = workPeriod.LunchStartHour.Hour,
                   LunchEndHour = workPeriod.LunchEndHour.Hour,
                   EndHour = workPeriod.EndHour.Hour
               };
    }

    private Facility ConvertFacility(Application.API.Dtos.Facility inputFacility)
    {
        return new Facility
               {
                   Name = inputFacility.Name,
                   Address = inputFacility.Address
               };
    }
}
