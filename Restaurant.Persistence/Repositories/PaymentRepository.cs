using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId)
    {
        return await _dbSet
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.OrderId == orderId);
    }

    public async Task<IReadOnlyList<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
    {
        return await _dbSet
            .Include(x => x.Order)
            .Where(x => x.Status == status)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<decimal> GetDailyRevenueAsync(DateTime date)
    {
        return await _dbSet
            .Where(x => x.PaymentDate.Date == date.Date && x.Status == PaymentStatus.Completed)
            .SumAsync(x => x.Amount);
    }
}