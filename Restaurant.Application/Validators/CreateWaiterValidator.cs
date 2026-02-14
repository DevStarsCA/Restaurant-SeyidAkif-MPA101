using FluentValidation;
using Restaurant.Application.DTOs.WaiterDtos;

namespace Restaurant.Application.Validators;

public class CreateWaiterValidator : AbstractValidator<CreateWaiterDto>
{
    public CreateWaiterValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100).WithMessage("Ad boş ola bilməz.");
    }
}






