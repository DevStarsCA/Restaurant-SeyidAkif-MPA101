using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId);
    Task<IReadOnlyList<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
    Task<decimal> GetDailyRevenueAsync(DateTime date);
}
