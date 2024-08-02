namespace SlotService.Domain.UnitTests;

[TestFixture]
public class TimePeriodTest
{
    [TestCase(9, 9, TestName = "Try to create with start and end same time, throws exception")]
    [TestCase(9, 8, TestName = "Try to create with end before start, throws exception")]
    public void CannotSetTwoWrongTimes(int start, int end)
    {
        // Arrange
        var startTime = new TimeOnly(start, 0, 0);
        var endTime = new TimeOnly(end, 0, 0);

        // Act / Assert
        Assert.Throws<BadTimePeriodException>(() => new TimePeriod(startTime, endTime));
    }
}
