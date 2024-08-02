using SlotService.Application.API;
using SlotService.Application.API.Dtos;
using SlotService.Application.API.Errors;
using SlotService.Application.Validators;


namespace SlotService.Application.UnitTests.Validators;

[TestFixture]
public class BookSlotCommandValidatorTest
{
    private readonly BookSlotCommandValidator _validator = new();

    [Test]
    public void Validate_WithOldDate_Fails()
    {
        // Arrange
        var query = new BookSlotCommand
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now).AddDays(-1),
                        Patient = new Patient(),
                        Start = TimeOnly.MinValue,
                        End = TimeOnly.MaxValue
                    };

        // Act
        var result = _validator.Validate(query);

        Assert.IsFalse(result.IsSuccess);
        Assert.That(result.Errors.First(), Is.TypeOf<SlotInThePastError>());
    }

    [Test]
    public void Validate_WithCurrentDate_Succeeds()
    {
        // Arrange
        var query = new BookSlotCommand
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Patient = new Patient(),
                        Start = TimeOnly.MinValue,
                        End = TimeOnly.MaxValue
                    };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void Validate_WithStartAfterEnd_Fails()
    {
        // Arrange
        var query = new BookSlotCommand
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Patient = new Patient(),
                        Start = TimeOnly.MaxValue,
                        End = TimeOnly.MinValue
                    };

        // Act
        var result = _validator.Validate(query);

        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<StartAfterEndError>());
    }
}
