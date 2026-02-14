using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.PaymentDtos;

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public string PaymentTypeText => PaymentType.ToString();
    public PaymentStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
}

