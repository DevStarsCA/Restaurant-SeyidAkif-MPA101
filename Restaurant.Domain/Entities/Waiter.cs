using Restaurant.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;
public class Waiter : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign Keys
    public string? AppUserId { get; set; }

    // Navigation Properties
    public AppUser? AppUser { get; set; }
    public ICollection<WaiterTable> WaiterTables { get; set; } = new List<WaiterTable>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
