using Restaurant.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class Basket : BaseEntity
{
    public string? SessionId { get; set; }

    public Guid TableId { get; set; }
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public Table Table { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
