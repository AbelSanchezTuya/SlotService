using SlotService.Application.API;
using SlotService.Domain;
using DaySchedule = SlotService.Application.API.Dtos.DaySchedule;
using Facility = SlotService.Application.API.Dtos.Facility;
using Slot = SlotService.Application.API.Dtos.Slot;
using WorkPeriod = SlotService.Application.API.Dtos.WorkPeriod;


namespace SlotService.Application.Mappers;

public class GetWeekAvailabilityResponseMapper
    : IMapper<GetWeekAvailabilityResponse, IWeekSchedule>
{
    public GetWeekAvailabilityResponse Map(IWeekSchedule input)
    {
        var response = new GetWeekAvailabilityResponse
                       {
                           Facility = ConvertFacility(input.Facility),
                           SlotDurationMinutes
                               = input.SlotDurationMinutes,
                           DaysSchedule = []
                       };
        foreach (var daySchedule in input.GetAvailability())
        {
            var convertedDaySchedule = ConvertDaySchedule(daySchedule);
            response.DaysSchedule.Add(convertedDaySchedule);
        }

        return response;
    }

    private DaySchedule ConvertDaySchedule(Domain.DaySchedule daySchedule)
    {
        return new DaySchedule
               {
                   Name = daySchedule.DayOfWeek,
                   WorkPeriod = ConvertWorkPeriod(daySchedule.WorkPeriod),
                   BusySlots = ConvertBusySlots(daySchedule.BusySlots).ToList()
               };
    }

    private IEnumerable<Slot> ConvertBusySlots(List<Domain.Slot> dayScheduleBusySlots)
    {
        foreach (var slot in dayScheduleBusySlots)
        {
            yield return new Slot
                         {
                             Date = slot.Date,
                             Start = slot.Start,
                             End = slot.End
                         };
        }
    }

    private WorkPeriod ConvertWorkPeriod(Domain.WorkPeriod workPeriod)
    {
        return new WorkPeriod
               {
                   StartHour = workPeriod.MorningShift.Start,
                   LunchStartHour = workPeriod.MorningShift.End,
                   LunchEndHour = workPeriod.AfternoonShift.Start,
                   EndHour = workPeriod.AfternoonShift.End
               };
    }

    private Facility ConvertFacility(Domain.Facility inputFacility)
    {
        return new Facility
               {
                   Name = inputFacility.Name,
                   Address = inputFacility.Address
               };
    }
}
