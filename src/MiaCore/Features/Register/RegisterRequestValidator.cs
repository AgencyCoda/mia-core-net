using FluentValidation;
using MediatR;

namespace MiaCore.Features.Register
{
    internal class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(25);

            RuleFor(x => x.Fullname)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(200);

            RuleFor(x => x.IdentificationFrontUrl)
            .NotEmpty()
            .NotNull()
            .When(x => x.InstitutionRegistration);
        }
    }
}