using System.Globalization;
using System.Linq.Expressions;
using FluentValidation;
using SlotService.API.REST.Model;


namespace SlotService.API.REST.Validators;

public class AppointmentValidator : AbstractValidator<Appointment>
{
    public AppointmentValidator()
    {
        RuleForDate("Start", x => x.Start);
        RuleForDate("End", x => x.End);
        RuleFor(x => x.Patient)
           .Cascade(CascadeMode.Stop)
           .NotNull()
           .DependentRules(
                () => { RuleFor(x => x.Patient).SetValidator(new PatientValidator()); });
    }

    private void RuleForDate(string text, Expression<Func<Appointment, string>> func)
    {
        RuleFor(func)
           .NotEmpty()
           .NotNull()
           .DependentRules(
                () =>
                {
                    RuleFor(func)
                       .Must(BeAValidDate)
                       .WithMessage("Invalid date format. Expected yyyy-MM-dd HH:mm:ss");
                });
    }

    private bool BeAValidDate(string incomingDate)
    {
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";

        return DateTime.TryParseExact(
            incomingDate,
            dateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }
}
