using System.Collections.Concurrent;
using FluentResults;
using SlotService.Application.API.Errors;


namespace SlotService.Application.API.Common;

public class MessageDispatcher : IMessageDispatcher
{
    private readonly ConcurrentDictionary<Type, IHandler> _handlers = [];

    public MessageDispatcher RegisterHandler<TMessage>(IHandler handler)
    {
        _handlers[typeof(TMessage)] = handler;

        return this;
    }

    public Task<IResultBase> Dispatch(IMessage request)
    {
        if (_handlers.TryGetValue(request.GetType(), out var handler))
        {
            return Task.Run(() => handler.Handle(request));
        }

        return Task.FromResult<IResultBase>(Result.Fail(new DispatchError(nameof(IMessage))));
    }
}
