using FluentValidation;
using Restaurant.Application.DTOs.ChatDtos;

namespace Restaurant.Application.Validators;

public class SendMessageValidator : AbstractValidator<SendMessageDto>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.TableId).NotEmpty().WithMessage("Masa seçilməlidir.");
        RuleFor(x => x.Message).NotEmpty().MaximumLength(500).WithMessage("Mesaj boş ola bilməz.");
    }
}






