using DocumentManagementSystem.Dtos;
using FluentValidation;

namespace DocumentManagementSystem.Business.ValidationRules
{
    public class AppUserUpdateDtoValidator : AbstractValidator<AppUserUpdateDto>
    {
        public AppUserUpdateDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.DeparmentId).NotEmpty();

        }
    }
}
