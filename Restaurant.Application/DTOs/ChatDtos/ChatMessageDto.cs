using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.ChatDtos;

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public ChatType ChatType { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public bool IsFromTable { get; set; }
    public Guid TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
}
