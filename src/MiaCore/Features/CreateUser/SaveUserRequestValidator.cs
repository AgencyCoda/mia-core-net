using FluentValidation;

namespace MiaCore.Features.CreateUser
{
    public class SaveUserRequestValidator : AbstractValidator<SaveUserRequest>
    {
        public SaveUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .MaximumLength(25)
                .When(x => !string.IsNullOrEmpty(x.Password));

            RuleFor(x => x.Firstname)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.Lastname)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);
        }
    }
}