using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOs.ChatDtos;

public class SendMessageDto
{
    public Guid TableId { get; set; }
    public string Message { get; set; } = string.Empty;
    public ChatType ChatType { get; set; }
    public bool IsFromTable { get; set; }
    public Guid? WaiterId { get; set; }
}
