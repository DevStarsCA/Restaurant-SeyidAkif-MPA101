using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOs.OrderDtos;

public class UpdateOrderStatusDto
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
}
