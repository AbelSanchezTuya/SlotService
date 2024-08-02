using System.Globalization;


namespace SlotService.Domain;

public class WeekSchedule : IWeekSchedule
{
    private readonly List<DaySchedule> _weekDays = new(7);

    public WeekSchedule(DateOnly weekDay, int slotDurationMinutes, Facility facility)
    {
        Facility = facility;
        SlotDurationMinutes = slotDurationMinutes;
        (WeekNumber, Year) = GetWeekNumberAndRelatedYear(weekDay);
        SetWeekBoundary(weekDay);
    }

    public int Year { get; }
    public int WeekNumber { get; }
    public DateOnly FistDay { get; private set; }
    public string Id => $"{Year}{WeekNumber}";
    public int SlotDurationMinutes { get; }
    public Facility Facility { get; }

    public List<DaySchedule> GetAvailability()
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        var (currentWeek, year) = GetWeekNumberAndRelatedYear(now);

        if (year > Year)
        {
            return [];
        }

        if (year == Year)
        {
            if (currentWeek > WeekNumber)
            {
                return [];
            }

            if (currentWeek == WeekNumber)
            {
                return _weekDays
                      .Where(x => x.DayOfWeek >= now.DayOfWeek)
                      .ToList();
            }

            if (currentWeek < WeekNumber)
            {
                return _weekDays;
            }
        }

        if (year < Year)
        {
            return _weekDays;
        }

        return [];
    }

    public bool CanAccomodate(Slot slot)
    {
        return SlotDurationMinutes == slot.Duration &&
               IsWithinWorkingHours(slot) &&
               !OverlapsExistingBusySlots(slot);
    }

    public void Book(Slot slot)
    {
        if (!CanAccomodate(slot))
        {
            throw new ArgumentException("verify slot can be accommodated");
        }
        var daySchedule = GetDaySchedule(slot.Date.DayOfWeek);
        daySchedule!.BusySlots.Add(slot);
    }

    private void SetWeekBoundary(DateOnly weekDay)
    {
        var diff = weekDay.DayOfWeek - DayOfWeek.Monday;
        if (diff < 0)
        {
            diff += 7;
        }
        FistDay = weekDay.AddDays(-diff);
    }

    private (int, int) GetWeekNumberAndRelatedYear(DateOnly date)
    {
        var calendar = CultureInfo.InvariantCulture.Calendar;

        var weekNumber = calendar.GetWeekOfYear(
            date.ToDateTime(TimeOnly.MinValue),
            CalendarWeekRule.FirstFullWeek,
            DayOfWeek.Monday);

        var year = date.Year;

        if (weekNumber == 1 &&
            date.Month == 12)
        {
            year++;
        }
        else if (weekNumber >= 52 &&
                 date.Month == 1)
        {
            year--;
        }

        return (weekNumber, year);
    }

    private bool OverlapsExistingBusySlots(Slot slot)
    {
        var daySchedule = GetDaySchedule(slot.Date.DayOfWeek);

        return daySchedule != null &&
               daySchedule.BusySlots.Count() != 0 &&
               daySchedule.BusySlots.Any(busySlot => slot.Overlaps(busySlot));
    }

    private bool IsWithinWorkingHours(Slot slot)
    {
        return GetDaySchedule(slot.Date.DayOfWeek)
           ?
          .WorkPeriod.AppliesFor(slot) ?? false;
    }

    private DaySchedule? GetDaySchedule(DayOfWeek dayOfWeek)
    {
        return _weekDays.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
    }

    public DaySchedule OpenDaySchedule(DayOfWeek weekDay, WorkPeriod workPeriod)
    {
        var daySchedule = new DaySchedule
                          {
                              DayOfWeek = weekDay,
                              WorkPeriod = workPeriod
                          };
        if (_weekDays.All(x => x.DayOfWeek != daySchedule.DayOfWeek))
        {
            _weekDays.Add(daySchedule);
        }

        return daySchedule;
    }
}
