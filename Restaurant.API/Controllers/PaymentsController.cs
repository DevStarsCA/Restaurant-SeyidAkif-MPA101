using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService_App _paymentService;

    public PaymentsController(IPaymentService_App paymentService)
    {
        _paymentService = paymentService;
    }

    [Authorize(Roles = "Cashier,Admin")]
    [HttpPost("cash/{orderId}")]
    public async Task<IActionResult> CreateCashPayment(Guid orderId)
    {
        var result = await _paymentService.CreateCashPaymentAsync(orderId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("online/{orderId}")]
    public async Task<IActionResult> CreateOnlinePayment(Guid orderId)
    {
        var result = await _paymentService.CreateOnlinePaymentAsync(orderId);
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