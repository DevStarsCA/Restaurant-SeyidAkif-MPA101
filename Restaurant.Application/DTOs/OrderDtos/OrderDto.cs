using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.OrderDtos;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string? WaiterName { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}
