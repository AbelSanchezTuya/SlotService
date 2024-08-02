using FluentResults;


namespace SlotService.Application.API.Common;

public interface IMessageDispatcher
{
    MessageDispatcher RegisterHandler<TMessage>(IHandler handler);

    Task<IResultBase> Dispatch(IMessage request);
}
