namespace SlotService.Domain;

public class WorkPeriod
{
    public required Shift MorningShift { get; set; }
    public required Shift AfternoonShift { get; set; }

    public bool AppliesFor(Slot slot)
    {
        return MorningShift.CanFit(slot) || AfternoonShift.CanFit(slot);
    }
}
