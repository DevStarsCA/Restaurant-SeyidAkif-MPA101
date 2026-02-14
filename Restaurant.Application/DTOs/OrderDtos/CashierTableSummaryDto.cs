namespace Restaurant.Application.DTOs.OrderDtos;

public class CashierTableSummaryDto
{
    public Guid TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int OrderCount { get; set; }
    public List<OrderDto> Orders { get; set; } = new();
}
