using AutoMapper;
using Microsoft.Extensions.Configuration;
using Restaurant.Application.Common;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.DTOs.PaymentDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;

public class PaymentAppService : IPaymentService_App
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IKapitalBankService _kapitalBankService;
    private readonly IConfiguration _configuration;

    public PaymentAppService(IUnitOfWork unitOfWork, IMapper mapper, IKapitalBankService paymentService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _kapitalBankService = paymentService;
        _configuration = configuration;
    }

    public async Task<ApiResponse<PaymentDto>> CreateCashPaymentAsync(Guid orderId)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
        if (order == null) return ApiResponse<PaymentDto>.FailResponse("Sifariş tapılmadı.");

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentType = PaymentType.Cash,
            Status = PaymentStatus.Completed
        };

        await _unitOfWork.Payments.AddAsync(payment);

        // Sifarişi tamamla
        order.Status = OrderStatus.Completed;
        _unitOfWork.Orders.Update(order);

        await CheckAndFreeTableAsync(order.TableId);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<PaymentDto>.SuccessResponse(_mapper.Map<PaymentDto>(payment), "Nağd ödəniş qəbul edildi.");
    }

    public async Task<ApiResponse<OnlinePaymentResponseDto>> CreateOnlinePaymentAsync(Guid orderId)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
        if (order == null) return ApiResponse<OnlinePaymentResponseDto>.FailResponse("Sifariş tapılmadı.");

        var redirectUrl = _configuration["KapitalBank:CallbackUrl"]!;

        var result = await _kapitalBankService.CreatePaymentAsync(
            order.TotalAmount,
            "AZN",
            $"Sifariş: {order.OrderNumber}",
            redirectUrl);

        if (!result.IsSuccess)
            return ApiResponse<OnlinePaymentResponseDto>.FailResponse($"Ödəniş yaradıla bilmədi: {result.ErrorMessage}");

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentType = PaymentType.Online,
            PurchaseId = result.PurchaseId,
            Password = result.Password,
            Secret = result.Secret,
            Status = PaymentStatus.Pending
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<OnlinePaymentResponseDto>.SuccessResponse(new OnlinePaymentResponseDto
        {
            PaymentId = payment.Id,
            PurchaseId = result.PurchaseId!.Value,
            HppUrl = result.HppUrl!,
            OrderNumber = order.OrderNumber
        }, "Ödəniş səhifəsinə yönləndirilir.");
    }

    public async Task<ApiResponse<PaymentDto>> CheckPaymentStatusAsync(Guid paymentId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null) return ApiResponse<PaymentDto>.FailResponse("Ödəniş tapılmadı.");

        if (!payment.PurchaseId.HasValue || string.IsNullOrEmpty(payment.Password))
            return ApiResponse<PaymentDto>.FailResponse("Bu ödəniş üçün Kapital Bank məlumatı yoxdur.");

        var result = await _kapitalBankService.GetPaymentInfoAsync(payment.PurchaseId.Value, payment.Password);

        if (!result.IsSuccess)
            return ApiResponse<PaymentDto>.FailResponse($"Status yoxlama uğursuz: {result.ErrorMessage}");

        // Kapital Bank statusunu bizim statusa çevir
        payment.Status = result.Status;

        if (result.Status == PaymentStatus.Completed)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId);
            if (order != null)
                await CheckAndFreeTableAsync(order.TableId);
        }

        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<PaymentDto>(payment);

        return payment.Status == PaymentStatus.Completed
            ? ApiResponse<PaymentDto>.SuccessResponse(dto, "Ödəniş tamamlandı.")
            : ApiResponse<PaymentDto>.SuccessResponse(dto, $"Ödəniş statusu: {payment.Status}");
    }

    public async Task<ApiResponse<PaymentDto>> RefundAsync(Guid paymentId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null) return ApiResponse<PaymentDto>.FailResponse("Ödəniş tapılmadı.");

        if (payment.Status != PaymentStatus.Completed)
            return ApiResponse<PaymentDto>.FailResponse("Yalnız tamamlanmış ödənişlər geri qaytarıla bilər.");

        if (payment.PaymentType == PaymentType.Online && payment.PurchaseId.HasValue && !string.IsNullOrEmpty(payment.Password))
        {
            var result = await _kapitalBankService.RefundPaymentAsync(payment.PurchaseId.Value, payment.Password, payment.Amount);
            if (!result.IsSuccess)
                return ApiResponse<PaymentDto>.FailResponse($"Geri qaytarma uğursuz: {result.ErrorMessage}");
        }

        payment.Status = PaymentStatus.Refunded;
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

    private async Task CheckAndFreeTableAsync(Guid tableId)
    {
        var unpaidOrders = await _unitOfWork.Orders.GetUnpaidOrdersByTableAsync(tableId);
        if (!unpaidOrders.Any())
        {
            var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
            if (table != null)
                table.Status = TableStatus.Available;
        }
    }
}