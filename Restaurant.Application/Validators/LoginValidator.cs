using FluentValidation;
using Restaurant.Application.DTOs.AuthDtos;

namespace Restaurant.Application.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("İstifadəçi adı boş ola bilməz.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifrə boş ola bilməz.");
    }
}






