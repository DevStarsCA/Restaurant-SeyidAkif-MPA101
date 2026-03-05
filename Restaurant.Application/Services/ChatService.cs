using AutoMapper;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.ChatDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;

namespace Restaurant.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ChatService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<ChatMessageDto>>> GetByTableAsync(Guid tableId, ChatType? chatType = null)
    {
        var messages = await _unitOfWork.ChatMessages.GetMessagesByTableAsync(tableId, chatType);
        return ApiResponse<List<ChatMessageDto>>.SuccessResponse(_mapper.Map<List<ChatMessageDto>>(messages));
    }

    public async Task<ApiResponse<ChatMessageDto>> SendMessageAsync(SendMessageDto dto)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(dto.TableId);
        if (table == null) return ApiResponse<ChatMessageDto>.FailResponse("Masa tapılmadı.");

        var message = new ChatMessage
        {
            TableId = dto.TableId,
            Message = System.Net.WebUtility.HtmlEncode(dto.Message),
            ChatType = dto.ChatType,
            IsFromTable = dto.IsFromTable,
            WaiterId = dto.WaiterId,
            SenderName = dto.IsFromTable ? table.Name : "Ofisiant"
        };

        await _unitOfWork.ChatMessages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ChatMessageDto>.SuccessResponse(_mapper.Map<ChatMessageDto>(message), "Göndərildi.");
    }

    public async Task<ApiResponse<bool>> MarkAsReadAsync(Guid tableId, ChatType chatType)
    {
        await _unitOfWork.ChatMessages.MarkAsReadAsync(tableId, chatType);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true);
    }
}