using SlotService.API.REST.Model;
using SlotService.API.REST.Validators;


namespace SlotService.API.REST.UnitTests.Validators;

[TestFixture]
public class AppointmentValidatorTest
{
    [SetUp]
    public void SetUp()
    {
        _slot = new Appointment
                {
                    End = "2024-06-23 11:12:13",
                    Start = "2024-06-23 11:12:13",
                    Comments = "Test",
                    Patient = new Patient
                              {
                                  Email = "test@email.com",
                                  Name = "Bilbo",
                                  SecondName = "Baggings",
                                  Phone = "3456"
                              }
                };
    }

    private Appointment _slot;

    [Test]
    public void Validate_WithoutPatient_Fails()
    {
        // Arrange
        var validator = new AppointmentValidator();
        _slot.Patient = null!;

        // Act
        var result = validator.Validate(_slot);

        // Assert
        Assert.IsFalse(result.IsValid);
    }

    [TestCase("", false, TestName = "EmptyDate")]
    [TestCase(
        "2024-07-31 12:34:56",
        true,
        TestName = "Valid format yyyy-MM-dd HH:mm:ss")]
    [TestCase(
        "31-07-2024 12:34:56",
        false,
        TestName = "Wrong format dd-MM-yyyy HH:mm:ss")]
    [TestCase(
        "2024/07/31 12:34:56",
        false,
        TestName = "Wrong format yyyy/MM/dd HH:mm:ss")]
    [TestCase(
        "2024-07-31T12:34:56",
        false,
        TestName = "Wrong format yyyy-MM-ddTHH:mm:ss")]
    [TestCase(
        "2024-07-31",
        false,
        TestName = "Wrong format yyyy-MM-dd (Missing time)")]
    [TestCase(
        "12:34:56",
        false,
        TestName = "Wrong format HH:mm:ss (Missing date)")]
    [TestCase(
        "07-31-2024 12:34:56",
        false,
        TestName = "Wrong format MM-dd-yyyy HH:mm:ss")]
    [TestCase(
        "2024-13-31 12:34:56",
        false,
        TestName = "Wrong format yyyy-MM-dd HH:mm:ss (Invalid month)")]
    [TestCase(
        "2024-07-32 12:34:56",
        false,
        TestName = "Wrong format yyyy-MM-dd HH:mm:ss (Invalid day)")]
    [TestCase(
        "2024-07-31 25:34:56",
        false,
        TestName = "Wrong format yyyy-MM-dd HH:mm:ss (Invalid hour)")]
    [TestCase(
        "2024-07-31 12:60:56",
        false,
        TestName = "Wrong format yyyy-MM-dd HH:mm:ss (Invalid minute)")]
    [TestCase(
        "2024-07-31 12:34:60",
        false,
        TestName = "Wrong format yyyy-MM-dd HH:mm:ss (Invalid second)")]
    [TestCase(
        "2024-07-31 12:34",
        false,
        TestName = "Wrong format yyyy-MM-dd HH:mm (Missing seconds)")]
    [TestCase(
        "20240731 12:34:56",
        false,
        TestName = "Wrong format yyyyMMdd HH:mm:ss (Missing dashes)")]
    public void Validate_Dates(string starDate, bool expectedResult)
    {
        // Arrange
        var validator = new AppointmentValidator();
        _slot.Start = starDate;

        // Act
        var result = validator.Validate(_slot);

        // Assert
        Assert.That(result.IsValid, Is.EqualTo(expectedResult));

        // Arrange
        _slot.End = starDate;

        // Act
        result = validator.Validate(_slot);

        // Assert
        Assert.That(result.IsValid, Is.EqualTo(expectedResult));
    }
}
