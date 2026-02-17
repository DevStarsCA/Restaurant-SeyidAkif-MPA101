using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs.BasketItemDtos;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet("{tableId}")]
    public async Task<IActionResult> GetByTable(Guid tableId)
    {
        var result = await _basketService.GetByTableAsync(tableId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddToBasket([FromBody] AddToBasketDto dto)
    {
        var result = await _basketService.AddToBasketAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateBasketItemDto dto)
    {
        var result = await _basketService.UpdateQuantityAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveItem(Guid id)
    {
        var result = await _basketService.RemoveItemAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpDelete("clear/{tableId}")]
    public async Task<IActionResult> ClearBasket(Guid tableId)
    {
        var result = await _basketService.ClearBasketAsync(tableId);
        return Ok(result);
    }
}