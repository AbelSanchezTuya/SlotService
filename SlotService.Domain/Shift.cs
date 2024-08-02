namespace SlotService.Domain;

public class Shift(TimeOnly start, TimeOnly end) : TimePeriod(start, end)
{
    public bool CanFit(Slot slot)
    {
        return slot.Start.IsBetween(Start, End) &&
               slot.End.IsBetween(Start, End);
    }
}
