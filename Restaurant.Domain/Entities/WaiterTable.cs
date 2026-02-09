using Restaurant.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class WaiterTable : BaseEntity
{
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    public Guid WaiterId { get; set; }
    public Guid TableId { get; set; }

    public Waiter Waiter { get; set; } = null!;
    public Table Table { get; set; } = null!;
}
