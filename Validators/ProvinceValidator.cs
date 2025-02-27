using FluentValidation;
using ProvEditorNET.Models;

namespace ProvEditorNET.Validators;

public class ProvinceValidator : AbstractValidator<Province>
{
    public ProvinceValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Province Name is required");
        RuleFor(p => p.Name).MinimumLength(5).WithMessage("Province Name must be at least 2 characters");
        RuleFor(p => p.ProvinceType).MustAsync(ValidateType).WithMessage("Province Type must be of SEA, LAND or CITY type");
    }

    private async Task<bool> ValidateType(string provinceType, CancellationToken cancellationToken)
    {
        Console.WriteLine("Fluent Validator");
        
        if(provinceType == "Land") return true;
        if(provinceType == "City") return true;
        if(provinceType == "Sea") return true;

        return false;
    }
}