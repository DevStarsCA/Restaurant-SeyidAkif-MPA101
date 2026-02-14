using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOs.OrderDtos;

public class KitchenOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string TableName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string? Note { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}
