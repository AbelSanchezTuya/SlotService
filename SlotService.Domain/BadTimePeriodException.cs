namespace SlotService.Domain;

public class BadTimePeriodException()
    : Exception("The end of a period should come after the start of that period.");
