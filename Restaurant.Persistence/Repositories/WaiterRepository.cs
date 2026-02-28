using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class WaiterRepository : GenericRepository<Waiter>, IWaiterRepository
{
    public WaiterRepository(AppDbContext context) : base(context) { }

    public async Task<Waiter?> GetWaiterWithTablesAsync(Guid waiterId)
    {
        return await _dbSet
            .Include(x => x.WaiterTables.Where(wt => wt.IsActive))
                .ThenInclude(wt => wt.Table)
            .FirstOrDefaultAsync(x => x.Id == waiterId);
    }

    public async Task AssignTableAsync(Guid waiterId, Guid tableId)
    {
        var wt = new WaiterTable { WaiterId = waiterId, TableId = tableId, IsActive = true, AssignedAt = DateTime.UtcNow };
        await _context.Set<WaiterTable>().AddAsync(wt);
    }
    public async Task<Waiter?> GetWaiterByUserIdAsync(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.AppUserId == userId);
    }

    public async Task<IReadOnlyList<Waiter>> GetActiveWaitersAsync()
    {
        return await _dbSet
            .Include(x => x.WaiterTables.Where(wt => wt.IsActive))
                .ThenInclude(wt => wt.Table)
            .Where(x => x.IsActive)
            .AsNoTracking()
            .ToListAsync();
    }
}