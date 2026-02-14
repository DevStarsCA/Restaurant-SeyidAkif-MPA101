namespace Restaurant.Application.DTOs.BasketItemDtos;

public class AddToBasketDto
{
    public Guid TableId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
