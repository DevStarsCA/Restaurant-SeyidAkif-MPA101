using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs.WaiterDtos;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Waiter")]
public class WaitersController : ControllerBase
{
    private readonly IWaiterService _waiterService;

    public WaitersController(IWaiterService waiterService)
    {
        _waiterService = waiterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _waiterService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("panel/{waiterId}")]
    public async Task<IActionResult> GetWaiterPanel(Guid waiterId)
    {
        var result = await _waiterService.GetWaiterPanelAsync(waiterId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateWaiterDto dto, IFormFile? image)
    {
        var result = await _waiterService.CreateAsync(dto, image);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateWaiterDto dto)
    {
        var result = await _waiterService.UpdateAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignToTable([FromBody] AssignWaiterToTableDto dto)
    {
        var result = await _waiterService.AssignToTableAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("unassign")]
    public async Task<IActionResult> UnassignFromTable([FromBody] AssignWaiterToTableDto dto)
    {
        var result = await _waiterService.UnassignFromTableAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}