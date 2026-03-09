using Restaurant.Domain.Entities.Common;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public ChatType ChatType { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public string SenderName { get; set; } = string.Empty;
    public bool IsFromTable { get; set; }

    public Guid TableId { get; set; }
    public Guid? WaiterId { get; set; }

    public Table Table { get; set; } = null!;
    public Waiter? Waiter { get; set; }
}