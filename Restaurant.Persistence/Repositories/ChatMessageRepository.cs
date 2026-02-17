using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ChatMessage>> GetMessagesByTableAsync(Guid tableId, ChatType? chatType = null)
    {
        var query = _dbSet
            .Include(x => x.Table)
            .Where(x => x.TableId == tableId);

        if (chatType.HasValue)
            query = query.Where(x => x.ChatType == chatType.Value);

        return await query
            .OrderBy(x => x.SentAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ChatMessage>> GetUnreadMessagesAsync(Guid tableId)
    {
        return await _dbSet
            .Where(x => x.TableId == tableId && !x.IsRead)
            .OrderBy(x => x.SentAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(Guid tableId, ChatType chatType)
    {
        var messages = await _dbSet
            .Where(x => x.TableId == tableId && x.ChatType == chatType && !x.IsRead)
            .ToListAsync();

        foreach (var msg in messages)
            msg.IsRead = true;
    }
}