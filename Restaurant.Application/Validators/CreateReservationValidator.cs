using FluentValidation;
using Restaurant.Application.DTOs.ReservationDtos;

namespace Restaurant.Application.Validators;

public class CreateReservationValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100).WithMessage("Ad boş ola bilməz.");
        RuleFor(x => x.CustomerPhone).NotEmpty().WithMessage("Telefon boş ola bilməz.");
        RuleFor(x => x.GuestCount).GreaterThan(0).WithMessage("Qonaq sayı 0-dan böyük olmalıdır.");
        RuleFor(x => x.ReservationDate).GreaterThan(DateTime.UtcNow).WithMessage("Tarix keçmiş ola bilməz.");
    }
}






