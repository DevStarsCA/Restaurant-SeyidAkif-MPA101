namespace Restaurant.Application.DTOs.OrderDtos;

public class CreateOrderDto
{
    public Guid TableId { get; set; }
    public string? Note { get; set; }
}
