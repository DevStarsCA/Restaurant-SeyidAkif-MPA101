using FluentValidation;
using Restaurant.Application.DTOs.ProductDtos;

namespace Restaurant.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Məhsul adı boş ola bilməz.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Qiymət 0-dan böyük olmalıdır.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Kateqoriya seçilməlidir.");
    }
}






