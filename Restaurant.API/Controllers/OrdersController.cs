using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Restaurant.Application.DTOs.OrderDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Hubs;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _orderHub;

    public OrdersController(IOrderService orderService, IHubContext<OrderHub> orderHub)
    {
        _orderService = orderService;
        _orderHub = orderHub;
    }
    [Authorize(Roles = "Admin,Waiter,Kitchen,Cashier")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _orderService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("table/{tableId}")]
    public async Task<IActionResult> GetByTable(Guid tableId)
    {
        var result = await _orderService.GetByTableAsync(tableId);
        return Ok(result);
    }

    [Authorize(Roles = "Kitchen,Admin")]
    [HttpGet("kitchen")]
    public async Task<IActionResult> GetKitchenOrders()
    {
        var result = await _orderService.GetKitchenOrdersAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Cashier,Admin")]
    [HttpGet("cashier-summary")]
    public async Task<IActionResult> GetCashierSummary()
    {
        var result = await _orderService.GetCashierSummaryAsync();
        return Ok(result);
    }

    [HttpDelete("{orderId}")]
    public async Task<IActionResult> CancelOrder(Guid orderId, [FromQuery] Guid? tableId)
    {
        // Əgər tableId varsa, sifarişin o masaya aid olduğunu yoxla
        if (tableId.HasValue)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order.Success && order.Data != null && order.Data.TableId != tableId.Value)
                return BadRequest(new { success = false, message = "Bu sifariş sizin masanıza aid deyil." });
        }
        var result = await _orderService.CancelOrderAsync(orderId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var result = await _orderService.CreateAsync(dto);
        if (!result.Success) return BadRequest(result);

        // SignalR - mətbəxə bildiriş
        await _orderHub.Clients.Group("Kitchen").SendAsync("NewOrder", result.Data!.OrderNumber, result.Data.TableName);

        return Ok(result);
    }

    [Authorize(Roles = "Kitchen,Waiter,Admin")]
    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusDto dto)
    {
        var result = await _orderService.UpdateStatusAsync(dto);
        if (!result.Success) return BadRequest(result);

        // SignalR - masaya bildiriş
        await _orderHub.Clients.Group($"Table_{result.Data!.TableId}")
            .SendAsync("OrderStatusChanged", result.Data.OrderNumber, result.Data.StatusText);

        return Ok(result);
    }
}