namespace SlotService.Domain;

public class TimePeriod
{
    public TimePeriod(TimeOnly start, TimeOnly end)
    {
        Start = start;
        if (end <= start)
        {
            throw new BadTimePeriodException();
        }
        End = end;
    }

    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
}
