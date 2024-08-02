namespace SlotService.Domain;

public class Slot(DateOnly date, TimeOnly start, TimeOnly end) : TimePeriod(start, end)
{
    public DateOnly Date => date;
    public int Duration => (int) (End - Start).TotalMinutes;

    public bool Overlaps(TimePeriod slot)
    {
        return slot.Start.IsBetween(Start, End) ||
               (slot.End.IsBetween(Start, End) && slot.End != Start);
    }
}
