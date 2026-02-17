using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class BasketItemRepository : GenericRepository<BasketItem>, IBasketItemRepository
{
    public BasketItemRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<BasketItem>> GetBasketItemsByTableIdAsync(Guid tableId)
    {
        return await _dbSet
            .Include(x => x.Product)
            .Where(x => x.TableId == tableId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task ClearBasketByTableIdAsync(Guid tableId)
    {
        var items = await _dbSet.Where(x => x.TableId == tableId).ToListAsync();
        _dbSet.RemoveRange(items);
    }
}