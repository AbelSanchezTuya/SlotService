using System.Collections;


namespace SlotService.Domain.UnitTests;

[TestFixture]
public class ShiftTest
{
    [SetUp]
    public void SetUp()
    {
        _shift = new Shift(Time, Time.AddHours(2));
    }

    private static readonly DateOnly Date = new();
    private static readonly TimeOnly Time = new(
        10,
        00,
        00);
    private Shift _shift;

    [TestCaseSource(nameof(CreateCases))]
    public bool Test_CanFit(Slot slot)
    {
        // Act
        var fits = _shift.CanFit(slot);

        // Assert
        return fits;
    }

    public static IEnumerable CreateCases()
    {
        yield return new TestCaseData(new Slot(Date, Time.AddHours(-2), Time.AddHours(-1)))
                    .Returns(false)
                    .SetName("Slot before shift");
        yield return new TestCaseData(new Slot(Date, Time.AddHours(-1), Time))
                    .Returns(false)
                    .SetName("Slot before shift with start shift and end slot boundaries matching");
        yield return new TestCaseData(new Slot(Date, Time.AddHours(-1), Time.AddHours(1)))
                    .Returns(false)
                    .SetName("Slot starts before shift and ends within shift");
        yield return new TestCaseData(new Slot(Date, Time, Time.AddHours(1)))
                    .Returns(true)
                    .SetName("Slot within shift");
        yield return new TestCaseData(new Slot(Date, Time.AddHours(1), Time.AddHours(3)))
                    .Returns(false)
                    .SetName("Slot starts within shift and ends after shift");
        yield return new TestCaseData(new Slot(Date, Time.AddHours(2), Time.AddHours(3)))
                    .Returns(false)
                    .SetName("Slot after shift with end shift and start slot boundaries matching");
        yield return new TestCaseData(new Slot(Date, Time.AddHours(3), Time.AddHours(4)))
                    .Returns(false)
                    .SetName("Slot after shift");
    }
}
