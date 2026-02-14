using FluentValidation;
using Restaurant.Application.DTOs.OrderDtos;

namespace Restaurant.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.TableId).NotEmpty().WithMessage("Masa seçilməlidir.");
    }
}






