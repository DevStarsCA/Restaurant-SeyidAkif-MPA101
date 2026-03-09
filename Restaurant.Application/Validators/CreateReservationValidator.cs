using FluentValidation;
using Restaurant.Application.DTOs.ReservationDtos;

namespace Restaurant.Application.Validators;

public class CreateReservationValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100).WithMessage("Ad boş ola bilməz.");
        RuleFor(x => x.CustomerPhone).NotEmpty().WithMessage("Telefon boş ola bilməz.");
        RuleFor(x => x.GuestCount).GreaterThan(0).LessThanOrEqualTo(20).WithMessage("Qonaq sayı 1-20 arasında olmalıdır.");
        RuleFor(x => x.ReservationDate) .GreaterThan(DateTime.UtcNow).WithMessage("Tarix keçmiş ola bilməz.")
           .LessThan(DateTime.UtcNow.AddDays(90)).WithMessage("Maksimum 90 gün sonraya rezervasiya edilə bilər.");
        RuleFor(x => x.CustomerPhone).Matches(@"^[\d\+\-\(\)\s]{7,20}$").WithMessage("Düzgün telefon nömrəsi daxil edin.");
        RuleFor(x => x.CustomerName).MaximumLength(100).WithMessage("Ad maksimum 100 simvol ola bilər.");
        RuleFor(x => x.Note).MaximumLength(500).WithMessage("Qeyd maksimum 500 simvol ola bilər.");
    }
}