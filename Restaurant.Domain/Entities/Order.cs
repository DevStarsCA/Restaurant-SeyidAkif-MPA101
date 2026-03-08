using Restaurant.Domain.Entities.Common;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;

    public Guid TableId { get; set; }
    public Guid? WaiterId { get; set; }

    public Table Table { get; set; } = null!;
    public Waiter? Waiter { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

}
