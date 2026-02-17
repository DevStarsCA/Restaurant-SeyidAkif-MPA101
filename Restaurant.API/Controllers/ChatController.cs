using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Restaurant.Application.DTOs.ChatDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Enums;
using Restaurant.Infrastructure.Hubs;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _chatHub;

    public ChatController(IChatService chatService, IHubContext<ChatHub> chatHub)
    {
        _chatService = chatService;
        _chatHub = chatHub;
    }

    [HttpGet("{tableId}")]
    public async Task<IActionResult> GetByTable(Guid tableId, [FromQuery] ChatType? chatType)
    {
        var result = await _chatService.GetByTableAsync(tableId, chatType);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        var result = await _chatService.SendMessageAsync(dto);
        if (!result.Success) return BadRequest(result);

        // SignalR - real-time mesaj
        await _chatHub.Clients.Group($"Chat_Table_{dto.TableId}")
            .SendAsync("ReceiveMessage", dto.TableId.ToString(), dto.Message, dto.IsFromTable);

        return Ok(result);
    }

    [HttpPut("read/{tableId}/{chatType}")]
    public async Task<IActionResult> MarkAsRead(Guid tableId, ChatType chatType)
    {
        var result = await _chatService.MarkAsReadAsync(tableId, chatType);
        return Ok(result);
    }
}