using FluentValidation;
using SlotService.API.REST.Model;


namespace SlotService.API.REST.Validators;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty()
           .NotNull()
           .EmailAddress()
           .WithMessage("Should provide a valid e-mail address");
        RuleFor(x => x.Phone)
           .NotEmpty()
           .NotNull();
        RuleFor(x => x.Name)
           .NotEmpty()
           .NotNull();
        RuleFor(x => x.SecondName)
           .NotEmpty()
           .NotNull();
    }
}
