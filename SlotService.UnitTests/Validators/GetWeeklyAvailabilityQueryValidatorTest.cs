using SlotService.Application.API;
using SlotService.Application.API.Errors;
using SlotService.Application.Validators;


namespace SlotService.Application.UnitTests.Validators;

[TestFixture]
public class GetWeeklyAvailabilityQueryValidatorTest
{
    private readonly GetWeeklyAvailabilityQueryValidator _validator = new();

    [Test]
    public void Validate_WithDateOneYearAgo_Fails()
    {
        // Arrange
        var query = new GetWeeklyAvailabilityQuery
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddYears(-1))
                    };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<WeekInThePastError>());
    }

    [Test]
    public void Validate_WithDateOneWeekAgo_Fails()
    {
        // Arrange
        var query = new GetWeeklyAvailabilityQuery
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-7))
                    };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<WeekInThePastError>());
    }

    [Test]
    public void Validate_WithDateLastSunday_Fails()
    {
        // Arrange
        var today = DateTime.Today;
        var daysToLastSunday = (int) today.DayOfWeek;
        var lastSunday = today.AddDays(-daysToLastSunday);
        var query = new GetWeeklyAvailabilityQuery { Date = DateOnly.FromDateTime(lastSunday) };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<WeekInThePastError>());
    }

    [Test]
    public void Validate_WithDateMondayOfCurrentWeek_Succeeds()
    {
        // Arrange
        var today = DateTime.Today;
        var daysToWeekMonday = ((int) today.DayOfWeek + 6) % 7;
        var lastSunday = today.AddDays(-daysToWeekMonday);
        var query = new GetWeeklyAvailabilityQuery { Date = DateOnly.FromDateTime(lastSunday) };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void Validate_WithDateSameDay_Succeeds()
    {
        // Arrange
        var query = new GetWeeklyAvailabilityQuery { Date = DateOnly.FromDateTime(DateTime.Now) };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void Validate_WithDateNextWeek_Succeeds()
    {
        // Arrange
        var query = new GetWeeklyAvailabilityQuery
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
                    };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsSuccess);
    }
}
