using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs.TableDTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Enums;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _tableService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _tableService.GetByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("qr/{qrCode}")]
    public async Task<IActionResult> GetByQRCode(string qrCode)
    {
        var result = await _tableService.GetByQRCodeAsync(qrCode);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(TableStatus status)
    {
        var result = await _tableService.GetByStatusAsync(status);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/qrcode")]
    public async Task<IActionResult> GetQRCode(Guid id, [FromQuery] string? baseUrl)
    {
        var result = await _tableService.GetQRCodeImageAsync(id, baseUrl);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTableDto dto)
    {
        var result = await _tableService.CreateAsync(dto);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTableDto dto)
    {
        var result = await _tableService.UpdateAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _tableService.DeleteAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}