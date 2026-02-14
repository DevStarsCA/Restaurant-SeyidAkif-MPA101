using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOs.ReservationDtos;

public class UpdateReservationStatusDto
{
    public Guid Id { get; set; }
    public ReservationStatus Status { get; set; }
}
