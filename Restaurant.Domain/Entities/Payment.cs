using Restaurant.Domain.Entities.Common;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class Payment : AuditableEntity
{
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public int? PurchaseId { get; set; }
    public string? Password { get; set; }
    public string? Secret { get; set; }
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public string? Note { get; set; }

    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

   
}