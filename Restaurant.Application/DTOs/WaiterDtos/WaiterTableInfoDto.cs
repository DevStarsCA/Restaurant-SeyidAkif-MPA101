namespace Restaurant.Application.DTOs.WaiterDtos;

public class WaiterTableInfoDto
{
    public Guid TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public bool HasReadyOrder { get; set; }
    public int ActiveOrderCount { get; set; }
}







