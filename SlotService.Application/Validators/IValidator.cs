using FluentResults;


namespace SlotService.Application.Validators;

public interface IValidator<in T>
{
    Result Validate(T value);
}
