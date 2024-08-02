using SlotService.Application.API;


namespace SlotService.Application.UnitTests.API;

[TestFixture]
public class BookSlotCommandBuilderTest
{
    [Test]
    public void Builds()
    {
        // Arrange
        var expectedDate = new DateTime(
            2024,
            6,
            6,
            7,
            8,
            9);
        var expectedStart = expectedDate.AddDays(1);
        var expectedEnd = expectedDate.AddDays(5);
        var date = expectedDate.ToString("yyyy-MM-dd HH:mm:ss");
        var start = expectedStart.ToString("yyyy-MM-dd HH:mm:ss");
        var end = expectedEnd.ToString("yyyy-MM-dd HH:mm:ss");
        const string email = "test@test.com";
        const string phone = "36778899";
        const string patientName = "Bilbo";
        const string patientSecondName = "Baggings";
        const string comments = "comments";

        // Act
        var command = new BookSlotCommandBuilder()
                     .SetDate(date)
                     .SetStart(start)
                     .SetEnd(end)
                     .SetComments(comments)
                     .SetEmail(email)
                     .SetPatientName(patientName)
                     .SetPatientSecondName(patientSecondName)
                     .SetPhone(phone)
                     .Build();

        // Assert
        Assert.NotNull(command);
        Assert.That(command.Comments, Is.EqualTo(comments));
        Assert.That(command.Date, Is.EqualTo(DateOnly.FromDateTime(expectedDate)));
        Assert.That(command.Start, Is.EqualTo(TimeOnly.FromDateTime(expectedStart)));
        Assert.That(command.End, Is.EqualTo(TimeOnly.FromDateTime(expectedEnd)));
        Assert.NotNull(command.Patient);
        Assert.That(command.Patient.Phone, Is.EqualTo(phone));
        Assert.That(command.Patient.Email, Is.EqualTo(email));
        Assert.That(command.Patient.Name, Is.EqualTo(patientName));
        Assert.That(command.Patient.SecondName, Is.EqualTo(patientSecondName));
    }
}
