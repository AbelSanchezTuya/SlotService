using System.Windows.Input;
using FluentResults;
using Moq;
using SlotService.Application.API.Common;
using SlotService.Application.API.Errors;
using SlotService.Application.Handlers;


namespace SlotService.Application.UnitTests.Handlers;

public class BaseHandlerTest
{
    [Test]
    public void Handle_WithWrongMessage_Fails()
    {
        // Arrange
        var wrongMessage = new Mock<ICommand>();
        var handler = new TestHandler();

        // Act
        var result = handler.Handle(wrongMessage.Object);

        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<BadMessageError>());
    }

    [Test]
    public void Handle_WithMessage_Fails()
    {
        // Arrange
        var wrongMessage = new Mock<IMessage>();
        var handler = new TestHandler();

        // Act
        var result = handler.Handle(wrongMessage.Object);

        Assert.IsTrue(result.IsSuccess);
    }
}


public class TestHandler : BaseHandler<IMessage>
{
    protected override IResultBase Handle(IMessage request)
    {
        return Result.Ok();
    }
}
