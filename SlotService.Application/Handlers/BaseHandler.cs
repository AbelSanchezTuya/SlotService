using FluentResults;
using SlotService.Application.API.Common;
using SlotService.Application.API.Errors;


namespace SlotService.Application.Handlers;

public abstract class BaseHandler<TRequest> : IHandler
{
    public IResultBase Handle<T>(T request)
    {
        if (request is TRequest convertedRequest)
        {
            return Handle(convertedRequest);
        }

        return Result.Fail(
            new BadMessageError(GetType().Name, typeof(TRequest).Name, typeof(T).Name));
    }

    protected abstract IResultBase Handle(TRequest request);
}
