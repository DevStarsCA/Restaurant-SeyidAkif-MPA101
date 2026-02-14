using FluentValidation;
using Restaurant.Application.DTOs.AuthDtos;

namespace Restaurant.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100).WithMessage("Ad boş ola bilməz.");
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(50).WithMessage("İstifadəçi adı boş ola bilməz.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Düzgün email daxil edin.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Şifrə ən azı 6 simvol olmalıdır.");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Şifrələr eyni olmalıdır.");
        RuleFor(x => x.Role).NotEmpty()
            .Must(r => new[] { "Admin", "Waiter", "Kitchen", "Cashier" }.Contains(r))
            .WithMessage("Rol Admin, Waiter, Kitchen və ya Cashier olmalıdır.");
    }
}






