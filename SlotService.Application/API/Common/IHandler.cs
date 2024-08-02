using FluentResults;


namespace SlotService.Application.API.Common;

public interface IHandler
{
    IResultBase Handle<T>(T request);
}
