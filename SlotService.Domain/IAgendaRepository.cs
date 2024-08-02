namespace SlotService.Domain;

public interface IAgendaRepository
{
    bool TryToGetWeekSchedule(DateOnly dayInTheWeek, out IWeekSchedule? weekSchedule);

    void UpdateWeekSchedule(IWeekSchedule weekSchedule);
}
