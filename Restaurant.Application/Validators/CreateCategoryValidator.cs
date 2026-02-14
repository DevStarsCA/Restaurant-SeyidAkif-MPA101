using FluentValidation;
using Restaurant.Application.DTOs.CategoryDtos;

namespace Restaurant.Application.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Kateqoriya adı boş ola bilməz.");
    }
}






