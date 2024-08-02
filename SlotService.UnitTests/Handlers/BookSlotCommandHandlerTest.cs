using FluentResults;
using Moq;
using SlotService.Application.API;
using SlotService.Application.API.Dtos;
using SlotService.Application.API.Errors;
using SlotService.Application.Handlers;
using SlotService.Application.Validators;
using SlotService.Domain;
using Slot = SlotService.Domain.Slot;


namespace SlotService.Application.UnitTests.Handlers;

[TestFixture]
public class BookSlotCommandHandlerTest
{
    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<BookSlotCommand>>();
        _weekScheduleMock = new Mock<IWeekSchedule>();
        _agendaMock = new Mock<IAgendaRepository>();
        _handler = new BookSlotCommandHandler(_validatorMock.Object, _agendaMock.Object);
    }

    private Mock<IValidator<BookSlotCommand>> _validatorMock;
    private Mock<IWeekSchedule> _weekScheduleMock;
    private Mock<IAgendaRepository> _agendaMock;
    private BookSlotCommandHandler _handler;

    [Test]
    public void Handle_NotValidCommand_Fails()
    {
        // Arrange
        _validatorMock
           .Setup(x => x.Validate(It.IsAny<BookSlotCommand>()))
           .Returns(Result.Fail("Test fail"));

        // Act
        var result = _handler.Handle(new BookSlotCommand { Patient = new Patient() });

        // Assert
        Assert.IsTrue(result.IsFailed);
        IWeekSchedule? variable;
        _agendaMock.Verify(
            x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out variable),
            Times.Never);
        _weekScheduleMock.Verify(x => x.CanAccomodate(It.IsAny<Slot>()), Times.Never);
        _weekScheduleMock.Verify(x => x.Book(It.IsAny<Slot>()), Times.Never);
    }

    [Test]
    public void Handle_NotAbleToFindSchedule_Fails()
    {
        // Arrange
        _validatorMock.Setup(x => x.Validate(It.IsAny<BookSlotCommand>()))
                      .Returns(Result.Ok);
        var weekSchedule = _weekScheduleMock.Object;
        _agendaMock.Setup(x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out weekSchedule))
                   .Returns(false);
        var command = new BookSlotCommand
                      {
                          Date = DateOnly.FromDateTime(DateTime.Now),
                          Start = TimeOnly.MinValue,
                          End = TimeOnly.MaxValue,
                          Patient = new Patient()
                      };

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<WeekNotAvailableError>());
        IWeekSchedule? variable;
        _agendaMock.Verify(
            x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out variable),
            Times.Once);
        _weekScheduleMock.Verify(x => x.CanAccomodate(It.IsAny<Slot>()), Times.Never);
        _weekScheduleMock.Verify(x => x.Book(It.IsAny<Slot>()), Times.Never);
    }

    [Test]
    public void Handle_CannotAccomodate_Fails()
    {
        // Arrange
        _validatorMock.Setup(x => x.Validate(It.IsAny<BookSlotCommand>()))
                      .Returns(Result.Ok);
        _weekScheduleMock.Setup(x => x.CanAccomodate(It.IsAny<Slot>())).Returns(false);
        var weekSchedule = _weekScheduleMock.Object;
        _agendaMock.Setup(x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out weekSchedule))
                   .Returns(true);
        var command = new BookSlotCommand
                      {
                          Date = DateOnly.FromDateTime(DateTime.Now),
                          Start = TimeOnly.MinValue,
                          End = TimeOnly.MaxValue,
                          Patient = new Patient()
                      };

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<NotAvailableSlotError>());
        IWeekSchedule? variable;
        _agendaMock.Verify(
            x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out variable),
            Times.Once);
        _weekScheduleMock.Verify(x => x.CanAccomodate(It.IsAny<Slot>()), Times.Once);
        _weekScheduleMock.Verify(x => x.Book(It.IsAny<Slot>()), Times.Never);
    }

    [Test]
    public void Handle_CanAccomodate_Books()
    {
        // Arrange
        _validatorMock.Setup(x => x.Validate(It.IsAny<BookSlotCommand>()))
                      .Returns(Result.Ok);
        _weekScheduleMock.Setup(x => x.CanAccomodate(It.IsAny<Slot>())).Returns(true);
        var weekSchedule = _weekScheduleMock.Object;
        _agendaMock.Setup(x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out weekSchedule))
                   .Returns(true);
        var command = new BookSlotCommand
                      {
                          Date = DateOnly.FromDateTime(DateTime.Now),
                          Start = TimeOnly.MinValue,
                          End = TimeOnly.MaxValue,
                          Patient = new Patient()
                      };

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        IWeekSchedule? variable;
        _agendaMock.Verify(
            x => x.TryToGetWeekSchedule(It.IsAny<DateOnly>(), out variable),
            Times.Once);
        _weekScheduleMock.Verify(x => x.CanAccomodate(It.IsAny<Slot>()), Times.Once);
        _weekScheduleMock.Verify(x => x.Book(It.IsAny<Slot>()), Times.Once);
    }
}
