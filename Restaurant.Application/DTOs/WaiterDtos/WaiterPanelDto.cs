namespace Restaurant.Application.DTOs.WaiterDtos;

public class WaiterPanelDto
{
    public Guid WaiterId { get; set; }
    public string WaiterName { get; set; } = string.Empty;
    public List<WaiterTableInfoDto> Tables { get; set; } = new();
}







