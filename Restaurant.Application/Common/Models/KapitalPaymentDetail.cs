using Restaurant.Domain.Enums;

namespace Restaurant.Application.Common.Models;

public class KapitalPaymentDetail
{
    public bool IsSuccess { get; set; }
    public int PurchaseId { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? ErrorMessage { get; set; }
}
