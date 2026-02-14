using Restaurant.Application.Common;
using Restaurant.Application.DTOs.ChatDtos;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Interfaces;

public interface IChatService
{
    Task<ApiResponse<List<ChatMessageDto>>> GetByTableAsync(Guid tableId, ChatType? chatType = null);
    Task<ApiResponse<ChatMessageDto>> SendMessageAsync(SendMessageDto dto);
    Task<ApiResponse<bool>> MarkAsReadAsync(Guid tableId, ChatType chatType);
}
