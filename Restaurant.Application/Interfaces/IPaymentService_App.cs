using Restaurant.Application.Common;
using Restaurant.Application.DTOs.PaymentDtos;

namespace Restaurant.Application.Interfaces;

public interface IPaymentService_App
{
    Task<ApiResponse<PaymentDto>> CreateCashPaymentAsync(Guid orderId);
    Task<ApiResponse<OnlinePaymentResponseDto>> CreateOnlinePaymentAsync(Guid orderId);
    Task<ApiResponse<PaymentDto>> CompleteByPurchaseIdAsync(int purchaseId);
    Task<ApiResponse<PaymentDto>> CheckPaymentStatusAsync(Guid paymentId);
    Task<ApiResponse<PaymentDto>> RefundAsync(Guid paymentId);
    Task<ApiResponse<DailyRevenueDto>> GetDailyRevenueAsync(DateTime date);
}
