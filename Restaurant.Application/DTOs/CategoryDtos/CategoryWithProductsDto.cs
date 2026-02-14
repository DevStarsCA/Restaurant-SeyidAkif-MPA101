using Restaurant.Application.DTOs.ProductDtos;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.DTOs.CategoryDtos;

public class CategoryWithProductsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<ProductDto> Products { get; set; } = new();
}






