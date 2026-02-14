namespace Restaurant.Application.DTOs.BasketItemDtos;

public class BasketSummaryDto
{
    public Guid TableId { get; set; }
    public List<BasketItemDto> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.SubTotal);
    public int TotalItems => Items.Sum(i => i.Quantity);
}
