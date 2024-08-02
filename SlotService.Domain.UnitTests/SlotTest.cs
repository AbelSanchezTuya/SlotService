using System.Collections;


namespace SlotService.Domain.UnitTests;

[TestFixture]
public class SlotTest
{
    [SetUp]
    public void SetUp()
    {
        _referenceSlot = new Slot(DateOnly.MinValue, Time, Time.AddHours(_slotDurationHours));
    }

    private static readonly TimeOnly Time = new(
        10,
        00,
        00);
    private Slot _referenceSlot;
    private readonly int _slotDurationHours = 2;

    [Test]
    public void TestDuration()
    {
        // Assert
        Assert.That(_referenceSlot.Duration, Is.EqualTo(_slotDurationHours * 60));
    }

    [TestCaseSource(nameof(CreateCases))]
    public bool Test_Overlaps(TimePeriod timePeriod)
    {
        // Act
        var overlaps = _referenceSlot.Overlaps(timePeriod);

        // Assert
        return overlaps;
    }

    public static IEnumerable CreateCases()
    {
        yield return new TestCaseData(new TimePeriod(Time.AddHours(-2), Time.AddHours(-1)))
                    .Returns(false)
                    .SetName("Slot before reference Slot");
        yield return new TestCaseData(new TimePeriod(Time.AddHours(-1), Time))
                    .Returns(false)
                    .SetName(
                         "Slot starts before reference Slot and ends when reference Slot starts");
        yield return new TestCaseData(new TimePeriod(Time.AddHours(-1), Time.AddHours(1)))
                    .Returns(true)
                    .SetName("Slot starts before reference Slot and ends within reference Slot");
        yield return new TestCaseData(new TimePeriod(Time, Time.AddHours(1)))
                    .Returns(true)
                    .SetName("Slot within reference Slot");
        yield return new TestCaseData(new TimePeriod(Time.AddHours(1), Time.AddHours(3)))
                    .Returns(true)
                    .SetName("Slot starts within reference Slot and ends after reference Slot");
        yield return new TestCaseData(new TimePeriod(Time.AddHours(2), Time.AddHours(3)))
                    .Returns(false)
                    .SetName("Slot starts when reference Slot ends and ends after");
        yield return new TestCaseData(new TimePeriod(Time.AddHours(3), Time.AddHours(4)))
                    .Returns(false)
                    .SetName("Slot after reference Slot");
    }
}
