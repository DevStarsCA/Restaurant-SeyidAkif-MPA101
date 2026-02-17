namespace Restaurant.Application.DTOs.PaymentDtos;

public class OnlinePaymentResponseDto
{
    public Guid PaymentId { get; set; }
    public int PurchaseId { get; set; }
    public string HppUrl { get; set; } = string.Empty;    // Kapital Bank ödəniş səhifəsi
    public string OrderNumber { get; set; } = string.Empty;
}

