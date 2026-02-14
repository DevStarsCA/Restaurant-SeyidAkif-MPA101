using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOs.TableDTOs;

public class UpdateTableDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public TableStatus Status { get; set; }
}