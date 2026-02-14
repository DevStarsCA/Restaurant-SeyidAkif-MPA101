namespace Restaurant.Application.DTOs.ProductDtos;

public class UpdateProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public int PreparationTimeMinutes { get; set; }
    public Guid CategoryId { get; set; }
}
