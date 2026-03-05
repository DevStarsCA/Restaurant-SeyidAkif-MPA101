using FluentValidation;
using Restaurant.Application.DTOs.BasketItemDtos;

namespace Restaurant.Application.Validators;

public class AddToBasketValidator : AbstractValidator<AddToBasketDto>
{
    public AddToBasketValidator()
    {
        RuleFor(x => x.TableId).NotEmpty().WithMessage("Masa seçilməlidir.");
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Məhsul seçilməlidir.");
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(50).WithMessage("Miqdar 1-50 arasında olmalıdır.");
    }
}






