using System.Collections.Concurrent;
using System.Globalization;
using SlotService.Domain;


namespace SlotService.Storage;

public class InMemoryAgendaRepository : IAgendaRepository
{
    private static readonly Lazy<InMemoryAgendaRepository> _instance =
        new(() => new InMemoryAgendaRepository());
    private readonly ConcurrentDictionary<string, IWeekSchedule> _weekSchedules = new();

    private InMemoryAgendaRepository() { }

    public static InMemoryAgendaRepository Instance => _instance.Value;

    public bool TryToGetWeekSchedule(DateOnly dayInTheWeek, out IWeekSchedule? weekSchedule)
    {
        var (weekNumber, year) = GetWeekNumberAndRelatedYear(dayInTheWeek);
        var key = $"{year}{weekNumber}";

        return _weekSchedules.TryGetValue(key, out weekSchedule);
    }

    public void UpdateWeekSchedule(IWeekSchedule weekSchedule)
    {
        var key = weekSchedule.Id;
        _weekSchedules[key] = weekSchedule;
    }

    public void AddWeekSchedule(IWeekSchedule weekSchedule)
    {
        var key = weekSchedule.Id;
        _weekSchedules[key] = weekSchedule;
    }

    private (int weekNumber, int year) GetWeekNumberAndRelatedYear(DateOnly date)
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

    public void Clear()
    {
        _weekSchedules.Clear();
    }
}
