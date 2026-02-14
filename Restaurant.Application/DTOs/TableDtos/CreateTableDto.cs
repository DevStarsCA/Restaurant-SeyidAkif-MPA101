namespace Restaurant.Application.DTOs.TableDTOs;

public class CreateTableDto
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
}
