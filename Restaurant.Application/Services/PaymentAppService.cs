using AutoMapper;
using Restaurant.Application.Common;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.DTOs.PaymentDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;

namespace Restaurant.Application.Services;

public class PaymentAppService : IPaymentService_App
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPaymentService _paymentService;

    public PaymentAppService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _paymentService = paymentService;
    }

    public async Task<ApiResponse<PaymentDto>> CreateCashPaymentAsync(Guid orderId)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
        if (order == null) return ApiResponse<PaymentDto>.FailResponse("Sifariş tapılmadı.");

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentType = PaymentType.Cash
        };
        payment.MarkAsCompleted();

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<PaymentDto>.SuccessResponse(_mapper.Map<PaymentDto>(payment), "Nağd ödəniş qəbul edildi.");
    }

    public async Task<ApiResponse<OnlinePaymentResponseDto>> CreateOnlinePaymentAsync(Guid orderId)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
        if (order == null) return ApiResponse<OnlinePaymentResponseDto>.FailResponse("Sifariş tapılmadı.");

        var result = await _paymentService.CreatePaymentAsync(order.TotalAmount, order.OrderNumber, $"Sifariş: {order.OrderNumber}");
        if (!result.IsSuccess)
            return ApiResponse<OnlinePaymentResponseDto>.FailResponse($"Ödəniş yaradıla bilmədi: {result.ErrorMessage}");

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentType = PaymentType.Online,
            TransactionId = result.SessionId
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<OnlinePaymentResponseDto>.SuccessResponse(new OnlinePaymentResponseDto
        {
            PaymentId = payment.Id,
            PaymentUrl = result.PaymentUrl!,
            SessionId = result.SessionId!,
            OrderNumber = order.OrderNumber
        }, "Ödəniş səhifəsinə yönləndirilir.");
    }

    public async Task<ApiResponse<PaymentDto>> HandleCallbackAsync(string orderId, string sessionId)
    {
        var result = await _paymentService.CheckPaymentStatusAsync(orderId, sessionId);
        var payments = await _unitOfWork.Payments.GetAsync(p => p.TransactionId == sessionId);
        var payment = payments.FirstOrDefault();

        if (payment == null) return ApiResponse<PaymentDto>.FailResponse("Ödəniş tapılmadı.");

        if (result.IsSuccess)
            payment.MarkAsCompleted(result.TransactionId);
        else
        {
            payment.MarkAsFailed();
            payment.Note = result.ErrorMessage;
        }

        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<PaymentDto>(payment);
        return result.IsSuccess
            ? ApiResponse<PaymentDto>.SuccessResponse(dto, "Ödəniş tamamlandı.")
            : ApiResponse<PaymentDto>.FailResponse($"Ödəniş uğursuz: {result.ErrorMessage}");
    }

    public async Task<ApiResponse<PaymentDto>> RefundAsync(Guid paymentId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null) return ApiResponse<PaymentDto>.FailResponse("Ödəniş tapılmadı.");

        if (payment.PaymentType == PaymentType.Online && !string.IsNullOrEmpty(payment.TransactionId))
            await _paymentService.RefundPaymentAsync(payment.TransactionId, payment.Amount);

        payment.Refund();
        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<PaymentDto>.SuccessResponse(_mapper.Map<PaymentDto>(payment), "Geri qaytarıldı.");
    }

    public async Task<ApiResponse<DailyRevenueDto>> GetDailyRevenueAsync(DateTime date)
    {
        var revenue = await _unitOfWork.Payments.GetDailyRevenueAsync(date);
        var payments = await _unitOfWork.Payments.GetAsync(p => p.PaymentDate.Date == date.Date && p.Status == PaymentStatus.Completed);

        return ApiResponse<DailyRevenueDto>.SuccessResponse(new DailyRevenueDto
        {
            Date = date,
            TotalRevenue = revenue,
            TotalOrders = payments.Count,
            CashPayments = payments.Count(p => p.PaymentType == PaymentType.Cash),
            OnlinePayments = payments.Count(p => p.PaymentType == PaymentType.Online)
        });
    }
}
