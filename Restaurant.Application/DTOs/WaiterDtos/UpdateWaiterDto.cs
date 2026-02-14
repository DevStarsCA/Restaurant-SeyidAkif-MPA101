namespace Restaurant.Application.DTOs.WaiterDtos;

public class UpdateWaiterDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
}







