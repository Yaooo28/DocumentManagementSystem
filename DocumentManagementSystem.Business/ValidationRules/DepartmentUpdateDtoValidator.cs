using DocumentManagementSystem.Dtos;
using FluentValidation;

namespace DocumentManagementSystem.Business.ValidationRules
{
    public class DepartmentUpdateDtoValidator : AbstractValidator<DepartmentUpdateDto>
    {
        public DepartmentUpdateDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Definition).NotEmpty();
        }
    }
}
