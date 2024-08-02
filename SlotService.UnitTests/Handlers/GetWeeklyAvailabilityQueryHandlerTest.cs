using FluentResults;
using Moq;
using SlotService.Application.API;
using SlotService.Application.API.Errors;
using SlotService.Application.Handlers;
using SlotService.Application.Mappers;
using SlotService.Application.Validators;
using SlotService.Domain;
using Facility = SlotService.Application.API.Dtos.Facility;


namespace SlotService.Application.UnitTests.Handlers;

[TestFixture]
public class GetWeeklyAvailabilityQueryHandlerTest
{
    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<GetWeeklyAvailabilityQuery>>();
        _weekScheduleMock = new Mock<IWeekSchedule>();
        _agendaMock = new Mock<IAgendaRepository>();
        _mapperMock = new Mock<IMapper<GetWeekAvailabilityResponse, IWeekSchedule>>();
    }

    private Mock<IValidator<GetWeeklyAvailabilityQuery>> _validatorMock;
    private Mock<IWeekSchedule> _weekScheduleMock;
    private Mock<IAgendaRepository> _agendaMock;
    private Mock<IMapper<GetWeekAvailabilityResponse, IWeekSchedule>> _mapperMock;

    [Test]
    public void Handle_NotValidCommand_Fails()
    {
        // Arrange
        _validatorMock
           .Setup(x => x.Validate(It.IsAny<GetWeeklyAvailabilityQuery>()))
           .Returns(Result.Fail("Test fail"));
        var handler = new GetWeeklyAvailabilityQueryHandler(
            _validatorMock.Object,
            _mapperMock.Object,
            _agendaMock.Object);

        // Act
        var result = handler.Handle(new GetWeeklyAvailabilityQuery());

        // Assert
        Assert.IsTrue(result.IsFailed);
        IWeekSchedule? variable;
        _agendaMock.Verify(
            x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out variable),
            Times.Never);
        _weekScheduleMock.Verify(x => x.GetAvailability(), Times.Never);
        _mapperMock.Verify(x => x.Map(It.IsAny<IWeekSchedule>()), Times.Never);
    }

    [Test]
    public void Handle_WithNotExistingSchedule_Fails()
    {
        // Arrange
        _validatorMock
           .Setup(x => x.Validate(It.IsAny<GetWeeklyAvailabilityQuery>()))
           .Returns(Result.Ok);
        var weekSchedule = _weekScheduleMock.Object;
        _agendaMock.Setup(x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out weekSchedule))
                   .Returns(false);
        var handler = new GetWeeklyAvailabilityQueryHandler(
            _validatorMock.Object,
            _mapperMock.Object,
            _agendaMock.Object);

        // Act
        var result = handler.Handle(new GetWeeklyAvailabilityQuery());

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<WeekNotAvailableError>());
        IWeekSchedule? variable;
        _agendaMock.Verify(
            x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out variable),
            Times.Once);
        _weekScheduleMock.Verify(x => x.GetAvailability(), Times.Never);
        _mapperMock.Verify(x => x.Map(It.IsAny<IWeekSchedule>()), Times.Never);
    }

    [Test]
    public void Handle_WithQuery_Succeeds()
    {
        // Arrange
        _validatorMock
           .Setup(x => x.Validate(It.IsAny<GetWeeklyAvailabilityQuery>()))
           .Returns(Result.Ok);
        var weekSchedule = _weekScheduleMock.Object;
        _agendaMock.Setup(x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out weekSchedule))
                   .Returns(true);
        _mapperMock.Setup(x => x.Map(It.IsAny<IWeekSchedule>()))
                   .Returns(new GetWeekAvailabilityResponse { Facility = new Facility() });
        var handler = new GetWeeklyAvailabilityQueryHandler(
            _validatorMock.Object,
            _mapperMock.Object,
            _agendaMock.Object);

        // Act
        var result = handler.Handle(new GetWeeklyAvailabilityQuery());

        // Assert
        Assert.IsTrue(result.IsSuccess);
        IWeekSchedule? variable;
        _agendaMock.Verify(
            x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out variable),
            Times.Once);
        _mapperMock.Verify(x => x.Map(It.IsAny<IWeekSchedule>()), Times.Once);
    }
}
