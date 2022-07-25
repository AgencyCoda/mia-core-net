using System.Linq;
using FluentValidation;
using MiaCore.Utils;

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

            RuleFor(x => x.Fullname)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(200);

            RuleFor(x => x.Language)
                .Must(x => Configs.AvailableLanguages.Contains(x));
        }
    }
}