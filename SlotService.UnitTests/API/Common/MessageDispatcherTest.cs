using System.Windows.Input;
using FluentResults;
using Moq;
using SlotService.Application.API.Common;
using SlotService.Application.API.Errors;


namespace SlotService.Application.UnitTests.API.Common;

[TestFixture]
public class MessageDispatcherTest
{
    [Test]
    public async Task Dispatch_WithNonHandlerForMessage_Fails()
    {
        // Arrange
        var mockRequest = new Mock<IMessage>();
        var mockHandler = new Mock<IHandler>();
        var dispatcher = new MessageDispatcher();
        dispatcher.RegisterHandler<ICommand>(mockHandler.Object);

        // Act
        var result = await dispatcher.Dispatch(mockRequest.Object);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.That(result.Errors.First(), Is.TypeOf<DispatchError>());
        mockHandler.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Never);
    }

    [Test]
    public async Task Dispatch_WithNonHandlerForMessage_ReturnsResult()
    {
        // Arrange
        var mockHandler = new Mock<IHandler>();
        mockHandler.Setup(x => x.Handle(It.IsAny<IMessage>()))
                   .Returns(Result.Ok());
        var dispatcher = new MessageDispatcher();
        dispatcher.RegisterHandler<MockMessage>(mockHandler.Object);

        // Act
        var result = await dispatcher.Dispatch(new MockMessage());

        // Assert
        Assert.IsTrue(result.IsSuccess);
        mockHandler.Verify(x => x.Handle(It.IsAny<IMessage>()), Times.Once);
    }


    private class MockMessage : IMessage { }
}
