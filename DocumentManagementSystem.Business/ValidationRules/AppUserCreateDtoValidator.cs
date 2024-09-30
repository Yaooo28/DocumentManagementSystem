using DocumentManagementSystem.Dtos;
using FluentValidation;

namespace DocumentManagementSystem.Business.ValidationRules
{
    public class AppUserCreateDtoValidator : AbstractValidator<AppUserCreateDto>
    {
        public AppUserCreateDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.DeparmentId).NotEmpty();
        }
    }
}
