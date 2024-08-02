using SlotService.Domain;


namespace SlotService.UnitTests.Domain;

[TestFixture]
public class WorkPeriodTest
{
    [SetUp]
    public void SetUp()
    {
        _workDate = new DateOnly(2024, 6, 23);
        _workPeriod = new WorkPeriod
                      {
                          MorningShift = new Shift(
                              new TimeOnly(9, 0, 0),
                              new TimeOnly(14, 0, 0)),
                          AfternoonShift = new Shift(
                              new TimeOnly(16, 0, 0),
                              new TimeOnly(18, 0, 0))
                      };
    }

    private WorkPeriod _workPeriod;
    private DateOnly _workDate;

    [TestCase(
        10,
        11,
        ExpectedResult = true,
        TestName = "Slot within morning shift is valid")]
    [TestCase(
        16,
        17,
        ExpectedResult = true,
        TestName = "Slot within afternoon shift is valid")]
    [TestCase(
        7,
        8,
        ExpectedResult = false,
        TestName = "Slot before morning shift is not valid")]
    [TestCase(
        15,
        16,
        ExpectedResult = false,
        TestName = "Slot between shifts is not valid")]
    [TestCase(
        19,
        20,
        ExpectedResult = false,
        TestName = "Slot after afternoon shift is not valid")]
    [TestCase(
        8,
        10,
        ExpectedResult = false,
        TestName = "Slot partially before morning shift is not valid")]
    [TestCase(
        13,
        15,
        ExpectedResult = false,
        TestName = "Slot partially after morning shift is not valid")]
    [TestCase(
        11,
        17,
        ExpectedResult = false,
        TestName = "Slot overlapping both shift is not valid")]
    [TestCase(
        15,
        17,
        ExpectedResult = false,
        TestName = "Slot partially before afternoon shift is not valid")]
    [TestCase(
        17,
        19,
        ExpectedResult = false,
        TestName = "Slot partially after afternoon shift is not valid")]
    public bool Test_AppliesFor(int slotStartTime, int slotEndTime)
    {
        // Arrange
        var slot = new Slot(
            _workDate,
            new TimeOnly(slotStartTime, 0, 0),
            new TimeOnly(slotEndTime, 0, 0));

        // Act
        var applies = _workPeriod.AppliesFor(slot);

        // Assert
        return applies;
    }
}
