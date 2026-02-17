using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class TableRepository : GenericRepository<Table>, ITableRepository
{
    public TableRepository(AppDbContext context) : base(context) { }

    public async Task<Table?> GetByQRCodeAsync(string qrCode)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.QRCode == qrCode);
    }

    public async Task<Table?> GetTableWithOrdersAsync(Guid tableId)
    {
        return await _dbSet
            .Include(x => x.Orders).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(x => x.Id == tableId);
    }

    public async Task<IReadOnlyList<Table>> GetTablesByStatusAsync(TableStatus status)
    {
        return await _dbSet.Where(x => x.Status == status).ToListAsync();
    }
}
