using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.BasketItemDtos;

public class BasketItemDto
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal => ProductPrice * Quantity;
}
