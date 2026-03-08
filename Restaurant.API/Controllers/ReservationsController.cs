using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Restaurant.Application.DTOs.ReservationDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Hubs;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly IHubContext<OrderHub> _orderHub;

    public ReservationsController(IReservationService reservationService, IHubContext<OrderHub> orderHub)
    {
        _reservationService = reservationService;
        _orderHub = orderHub;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reservationService.GetAllAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
    {
        var result = await _reservationService.GetByDateAsync(date);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        var result = await _reservationService.CreateAsync(dto);
        if (!result.Success) return BadRequest(result);

        await _orderHub.Clients.Group("Admin").SendAsync("NewReservation", result.Data!.CustomerName);

        return Ok(result);
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateReservationStatusDto dto)
    {
        var result = await _reservationService.UpdateStatusAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _reservationService.DeleteAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}