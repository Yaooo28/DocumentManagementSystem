using DocumentManagementSystem.Dtos;
using FluentValidation;

namespace DocumentManagementSystem.Business.ValidationRules
{
    public class DepartmentCreateDtoValidator : AbstractValidator<DepartmentCreateDto>
    {
        public DepartmentCreateDtoValidator()
        {
            RuleFor(x => x.Definition).NotEmpty();
        }
    }
}
