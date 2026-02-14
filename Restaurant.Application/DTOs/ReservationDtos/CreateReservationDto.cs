namespace Restaurant.Application.DTOs.ReservationDtos;

public class CreateReservationDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public DateTime ReservationDate { get; set; }
    public int GuestCount { get; set; }
    public string? Note { get; set; }
    public Guid TableId { get; set; }
}
