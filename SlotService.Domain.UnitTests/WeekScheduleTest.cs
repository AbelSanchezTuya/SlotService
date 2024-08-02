using System.Collections;


namespace SlotService.Domain.UnitTests;

[TestFixture]
public class WeekScheduleTest
{
    [SetUp]
    public void SetUp()
    {
        _workPeriod = new WorkPeriod
                      {
                          MorningShift = new Shift(new TimeOnly(9, 0, 0), new TimeOnly(14, 0, 0)),
                          AfternoonShift = new Shift(new TimeOnly(16, 0, 0), new TimeOnly(18, 0, 0))
                      };
        _weekDay = new DateOnly(2024, 06, 10);
        _weekSchedule = new WeekSchedule(_weekDay, SlotDurationMinutes, new Facility());
        _weekSchedule.OpenDaySchedule(_weekDay.DayOfWeek, _workPeriod);
    }

    private const int SlotDurationMinutes = 20;
    private DateOnly _weekDay;
    private WeekSchedule _weekSchedule;
    private WorkPeriod _workPeriod;

    [Test]
    public void CanAccomodate_WithSlots_True()
    {
        // Arrange
        var slotStart = new TimeOnly(10, 0, 0);
        var slot = new Slot(_weekDay, slotStart, slotStart.AddMinutes(SlotDurationMinutes));

        // Act
        var canAccomodate = _weekSchedule.CanAccomodate(slot);

        // Assert
        Assert.IsTrue(canAccomodate);
    }

    [Test]
    public void CanAccomodate_WithSlotNotMatchingLength_False()
    {
        // Arrange
        var slotStart = new TimeOnly(10, 0, 0);
        var slot = new Slot(_weekDay, slotStart, slotStart.AddMinutes(SlotDurationMinutes + 2));

        // Act
        var canAccomodate = _weekSchedule.CanAccomodate(slot);

        // Assert
        Assert.IsFalse(canAccomodate);
    }

    [Test]
    public void CanAccomodate_OutOfWorkingOurs_False()
    {
        // Arrange
        var slotStart = new TimeOnly(8, 0, 0);
        var slot = new Slot(_weekDay, slotStart, slotStart.AddMinutes(SlotDurationMinutes));

        // Act
        var canAccomodate = _weekSchedule.CanAccomodate(slot);

        // Assert
        Assert.IsFalse(canAccomodate);
    }

    [Test]
    public void CanAccomodate_And_Book_OverlappingSlot_False()
    {
        // Accommodates and books
        var slotStart = new TimeOnly(10, 0, 0);
        var slot = new Slot(_weekDay, slotStart, slotStart.AddMinutes(SlotDurationMinutes));
        var canAccomodate = _weekSchedule.CanAccomodate(slot);
        Assert.IsTrue(canAccomodate);
        _weekSchedule.Book(slot);

        // Creates new slot for one minute later overlapping
        var overlappingSlotStart = slotStart.AddMinutes(1);
        var overlappingSlot = new Slot(
            _weekDay,
            overlappingSlotStart,
            overlappingSlotStart.AddMinutes(SlotDurationMinutes));
        canAccomodate = _weekSchedule.CanAccomodate(overlappingSlot);
        Assert.IsFalse(canAccomodate);
    }

    [Test]
    public void Book_OverlappingSlot_ThrowException()
    {
        // Accommodates and books
        var slotStart = new TimeOnly(10, 0, 0);
        var slot = new Slot(_weekDay, slotStart, slotStart.AddMinutes(SlotDurationMinutes));
        var canAccomodate = _weekSchedule.CanAccomodate(slot);
        Assert.IsTrue(canAccomodate);
        _weekSchedule.Book(slot);

        // Creates new slot for one minute later overlapping
        var overlappingSlotStart = slotStart.AddMinutes(1);
        var overlappingSlot = new Slot(
            _weekDay,
            overlappingSlotStart,
            overlappingSlotStart.AddMinutes(SlotDurationMinutes));
        Assert.Throws<ArgumentException>(() => _weekSchedule.Book(overlappingSlot));
    }

    [Test]
    public void OpenDaySchedule_OpensSlots()
    {
        // Arrange
        var dayToOpenSchedule = _weekDay.AddDays(1);
        var slotStart = new TimeOnly(10, 0, 0);
        var slot = new Slot(
            dayToOpenSchedule,
            slotStart,
            slotStart.AddMinutes(SlotDurationMinutes));

        // No slots available
        var canAccomodate = _weekSchedule.CanAccomodate(slot);
        Assert.IsFalse(canAccomodate);

        // Open schedule
        _weekSchedule.OpenDaySchedule(dayToOpenSchedule.DayOfWeek, _workPeriod);

        // Act
        canAccomodate = _weekSchedule.CanAccomodate(slot);

        // Assert
        Assert.IsTrue(canAccomodate);
    }

    [Test]
    public void GetAvailability_WhenWeekScheduleIsFromSameWeekPreviousYear_Empty()
    {
        // Arrange
        var weekMonday = new DateOnly(2024, 7, 8);
        var weekSchedule = new WeekSchedule(weekMonday, SlotDurationMinutes, new Facility());
        AddWeekSchedules(weekSchedule, weekMonday);

        // Act
        var availability = weekSchedule.GetAvailability();

        // Assert
        Assert.IsEmpty(availability);
    }

    [Test]
    public void GetAvailability_WhenWeekScheduleIsFromNextYear_HasAvailability()
    {
        // Arrange
        var weekMonday = new DateOnly(DateTime.Today.Year + 1, 7, 8);
        var weekSchedule = new WeekSchedule(weekMonday, SlotDurationMinutes, new Facility());
        AddWeekSchedules(weekSchedule, weekMonday);

        // Act
        var availability = weekSchedule.GetAvailability();

        // Assert
        Assert.That(availability.Count, Is.EqualTo(5));
    }

    [Test]
    public void GetAvailability_WhenWeekScheduleIsFromPreviousYear_HasAvailability()
    {
        // Arrange
        var weekMonday = new DateOnly(DateTime.Today.Year - 1, 7, 8);
        var weekSchedule = new WeekSchedule(weekMonday, SlotDurationMinutes, new Facility());
        AddWeekSchedules(weekSchedule, weekMonday);

        // Act
        var availability = weekSchedule.GetAvailability();

        // Assert
        Assert.That(availability.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetAvailability_WhenWeekScheduleIsFromNextWeek_HasAvailability()
    {
        // Arrange
        var today = DateTime.Today;
        var daysUntilMonday = (DayOfWeek.Monday - today.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0)
        {
            daysUntilMonday = 7;
        }
        var nextMonday = today.AddDays(daysUntilMonday);
        var weekMonday = DateOnly.FromDateTime(nextMonday);
        var weekSchedule = new WeekSchedule(weekMonday, SlotDurationMinutes, new Facility());
        AddWeekSchedules(weekSchedule, weekMonday);

        // Act
        var availability = weekSchedule.GetAvailability();

        // Assert
        Assert.That(availability.Count, Is.EqualTo(5));
    }

    [Test]
    public void GetAvailability_WhenWeekScheduleIsFromCurrentWeek_HasPartial()
    {
        var today = DateTime.Today;
        var daysFromMonday = today.DayOfWeek - DayOfWeek.Monday;
        if (daysFromMonday == -1)
        {
            daysFromMonday = 7;
        }
        var monday = today.AddDays(-daysFromMonday);
        var weekMonday = DateOnly.FromDateTime(monday);
        var weekSchedule = new WeekSchedule(weekMonday, SlotDurationMinutes, new Facility());
        AddWeekSchedules(weekSchedule, weekMonday);

        // Act
        var availability = weekSchedule.GetAvailability();

        // Assert
        var daysWithSchedule = 5;
        var sundayOffset = 1; // sunday is first day of week and generates offset for calculation
        var daysAvailable = daysWithSchedule - (int) today.DayOfWeek + sundayOffset;
        daysAvailable = daysAvailable < 0 ? 0 : daysAvailable;
        Assert.That(availability.Count, Is.EqualTo(daysAvailable));
    }

    [Test]
    [TestCaseSource(nameof(CreateCases))]
    public string GeneratesId(DateOnly date)
    {
        // Arrange
        var weekSchedule = new WeekSchedule(date, SlotDurationMinutes, new Facility());

        // Act / Assert
        return weekSchedule.Id;
    }

    private void AddWeekSchedules(WeekSchedule weekSchedule, DateOnly weekMonday)
    {
        weekSchedule.OpenDaySchedule(weekMonday.DayOfWeek, _workPeriod);
        weekSchedule.OpenDaySchedule(weekMonday.AddDays(1).DayOfWeek, _workPeriod);
        weekSchedule.OpenDaySchedule(weekMonday.AddDays(2).DayOfWeek, _workPeriod);
        weekSchedule.OpenDaySchedule(weekMonday.AddDays(3).DayOfWeek, _workPeriod);
        weekSchedule.OpenDaySchedule(weekMonday.AddDays(4).DayOfWeek, _workPeriod);
    }

    public static IEnumerable CreateCases()
    {
        yield return new TestCaseData(new DateOnly(2024, 12, 26))
                    .Returns("202452")
                    .SetName("Last previous week of the year");
        yield return new TestCaseData(new DateOnly(2024, 12, 31))
                    .Returns("202453")
                    .SetName("Last week of the year, with last day");
        yield return new TestCaseData(new DateOnly(2025, 1, 1))
                    .Returns("202453")
                    .SetName("Last week of the year, with first day of new");
        yield return new TestCaseData(new DateOnly(2025, 1, 8))
                    .Returns("20251")
                    .SetName("Second week of the year");
    }
}
