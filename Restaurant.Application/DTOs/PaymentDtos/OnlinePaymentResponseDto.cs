namespace Restaurant.Application.DTOs.PaymentDtos;

public class OnlinePaymentResponseDto
{
    public Guid PaymentId { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
}







