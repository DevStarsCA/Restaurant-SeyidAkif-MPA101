using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Order>> GetOrdersByTableIdAsync(Guid tableId)
    {
        return await _dbSet
            .Include(x => x.OrderItems).ThenInclude(oi => oi.Product)
            .Include(x => x.Table)
            .Include(x => x.Waiter)
            .Where(x => x.TableId == tableId)
            .OrderByDescending(x => x.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .Include(x => x.OrderItems).ThenInclude(oi => oi.Product)
            .Include(x => x.Table)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetActiveOrdersAsync()
    {
        return await _dbSet
            .Include(x => x.OrderItems).ThenInclude(oi => oi.Product)
            .Include(x => x.Table)
            .Where(x => x.Status != OrderStatus.Cancelled && x.Status != OrderStatus.Delivered)
            .OrderBy(x => x.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
    {
        return await _dbSet
            .Include(x => x.OrderItems).ThenInclude(oi => oi.Product)
            .Include(x => x.Table)
            .Include(x => x.Waiter)
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<IReadOnlyList<Order>> GetUnpaidOrdersByTableAsync(Guid tableId)
    {
        return await _dbSet
            .Include(x => x.Payments)
            .Where(x => x.TableId == tableId
                && x.Status != OrderStatus.Cancelled
                && !x.Payments.Any(p => p.Status == PaymentStatus.Completed))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<decimal> GetTotalAmountByTableAsync(Guid tableId)
    {
        return await _dbSet
            .Where(x => x.TableId == tableId
                && x.Status != OrderStatus.Cancelled
                && !x.Payments.Any(p => p.Status == PaymentStatus.Completed))
            .SumAsync(x => x.TotalAmount);
    }
}