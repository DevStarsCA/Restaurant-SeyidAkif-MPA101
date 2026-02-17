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
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }

    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    //public void MarkAsCompleted(string? transactionId = null)
    //{
    //    Status = PaymentStatus.Completed;
    //    TransactionId = transactionId;
    //}

    //public void MarkAsFailed()
    //{
    //    Status = PaymentStatus.Failed;
    //}

    //public void Refund()
    //{
    //    if (Status != PaymentStatus.Completed)
    //        throw new InvalidOperationException("Yalnız tamamlanmış ödənişlər geri qaytarıla bilər.");

    //    Status = PaymentStatus.Refunded;
    //}
}
