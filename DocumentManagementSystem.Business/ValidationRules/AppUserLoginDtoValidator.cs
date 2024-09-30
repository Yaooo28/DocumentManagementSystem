using DocumentManagementSystem.Dtos;
using FluentValidation;

namespace DocumentManagementSystem.Business.ValidationRules
{
    public class AppUserLoginDtoValidator : AbstractValidator<AppUserLoginDto>
    {
        public AppUserLoginDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username couldnt'be empty.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password couldnt'be empty.");
        }
    }
}
