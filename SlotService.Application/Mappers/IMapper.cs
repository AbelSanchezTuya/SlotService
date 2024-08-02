namespace SlotService.Application.Mappers;

public interface IMapper<out TResult, in TSource>
{
    TResult Map(TSource input);
}
