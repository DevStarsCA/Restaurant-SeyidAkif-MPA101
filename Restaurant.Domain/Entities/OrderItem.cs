using Restaurant.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Note { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;

    //public decimal GetSubTotal()
    //{
    //    return UnitPrice * Quantity;
    //}
}
