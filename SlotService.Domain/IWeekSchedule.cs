namespace SlotService.Domain;

public interface IWeekSchedule
{
    string Id { get; }
    Facility Facility { get; }
    int SlotDurationMinutes { get; }

    bool CanAccomodate(Slot slot);

    void Book(Slot slot);

    List<DaySchedule> GetAvailability();
}
