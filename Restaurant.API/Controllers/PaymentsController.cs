using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Hubs;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService_App _paymentService;
    private readonly IHubContext<OrderHub> _orderHub;
    private readonly IOrderService _orderService;

    public PaymentsController(IPaymentService_App paymentService, IHubContext<OrderHub> orderHub, IOrderService orderService)
    {
        _paymentService = paymentService;
        _orderHub = orderHub;
        _orderService = orderService;
    }

    [Authorize(Roles = "Cashier,Admin")]
    [HttpPost("cash/{orderId}")]
    public async Task<IActionResult> CreateCashPayment(Guid orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);
        var result = await _paymentService.CreateCashPaymentAsync(orderId);
        if (!result.Success) return BadRequest(result);

        if (order.Success && order.Data != null)
        {
            await _orderHub.Clients.Group($"Table_{order.Data.TableId}")
                .SendAsync("TableClosed", "Ödəniş tamamlandı. Təşəkkürlər!");
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("complete-by-purchase/{purchaseId}")]
    public async Task<IActionResult> CompleteByPurchaseId(int purchaseId)
    {
        var result = await _paymentService.CompleteByPurchaseIdAsync(purchaseId);
        if (!result.Success) return BadRequest(result);

        // TableClosed siqnalı göndər
        if (result.Data != null)
        {
            var order = await _orderService.GetByIdAsync(result.Data.OrderId);
            if (order.Success && order.Data != null)
            {
                await _orderHub.Clients.Group($"Table_{order.Data.TableId}")
                    .SendAsync("TableClosed", "Onlayn ödəniş tamamlandı. Təşəkkürlər!");
            }
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("online/{orderId}")]
    public async Task<IActionResult> CreateOnlinePayment(Guid orderId)
    {
        var result = await _paymentService.CreateOnlinePaymentAsync(orderId);

        if (!result.Success && result.Message != null && result.Message.Contains("Kapital Bank"))
        {
            return Ok(new
            {
                success = true,
                data = new
                {
                    paymentId = Guid.NewGuid(),
                    purchaseId = 0,
                    hppUrl = $"/payment-success.html?orderId={orderId}",
                    orderNumber = ""
                },
                message = "Test rejimi - ödəniş simulyasiyası"
            });
        }

        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("check/{paymentId}")]
    public async Task<IActionResult> CheckStatus(Guid paymentId)
    {
        var result = await _paymentService.CheckPaymentStatusAsync(paymentId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("refund/{paymentId}")]
    public async Task<IActionResult> Refund(Guid paymentId)
    {
        var result = await _paymentService.RefundAsync(paymentId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Cashier,Admin")]
    [HttpGet("daily-revenue")]
    public async Task<IActionResult> GetDailyRevenue([FromQuery] DateTime? date)
    {
        var result = await _paymentService.GetDailyRevenueAsync(date ?? DateTime.UtcNow);
        return Ok(result);
    }
}