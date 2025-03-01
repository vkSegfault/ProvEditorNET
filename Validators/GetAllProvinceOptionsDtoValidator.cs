using FluentValidation;
using ProvEditorNET.DTO;

namespace ProvEditorNET.Validators;

public class GetAllProvinceOptionsDtoValidator : AbstractValidator<GetAllProvincesOptionsDto>
{
    public GetAllProvinceOptionsDtoValidator()
    {
        RuleFor(o => o.page).GreaterThanOrEqualTo(0).WithMessage("Page must be greater than 0");
        RuleFor(o => o.pageSize).LessThanOrEqualTo(25).WithMessage("Page size must be between 0 and 25");
    }
}