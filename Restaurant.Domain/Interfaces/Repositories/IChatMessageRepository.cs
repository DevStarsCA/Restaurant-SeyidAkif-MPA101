using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface IChatMessageRepository : IGenericRepository<ChatMessage>
{
    Task<IReadOnlyList<ChatMessage>> GetMessagesByTableAsync(Guid tableId, ChatType? chatType = null);
    Task<IReadOnlyList<ChatMessage>> GetUnreadMessagesAsync(Guid tableId);
    Task MarkAsReadAsync(Guid tableId, ChatType chatType);
}







