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
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public Guid TableId { get; set; }
    public Guid? WaiterId { get; set; }

    public Table Table { get; set; } = null!;
    public Waiter? Waiter { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    //public Payment? Payment { get; set; }

    public void CalculateTotal()
    {
        TotalAmount = OrderItems.Sum(oi => oi.GetSubTotal());
    }

    public void MarkAsPreparing()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Yalnız gözləyən sifarişlər hazırlanmağa başlaya bilər.");

        Status = OrderStatus.Preparing;
    }

    public void MarkAsReady()
    {
        if (Status != OrderStatus.Preparing)
            throw new InvalidOperationException("Yalnız hazırlanan sifarişlər hazır olaraq işarələnə bilər.");

        Status = OrderStatus.Ready;
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Ready)
            throw new InvalidOperationException("Yalnız hazır sifarişlər çatdırılmış olaraq işarələnə bilər.");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Ready || Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Hazır və ya çatdırılmış sifarişlər ləğv edilə bilməz.");

        Status = OrderStatus.Cancelled;
    }

    public static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
}







