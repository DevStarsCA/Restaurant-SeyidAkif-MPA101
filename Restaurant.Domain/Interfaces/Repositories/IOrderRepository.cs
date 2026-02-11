using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IReadOnlyList<Order>> GetOrdersByTableIdAsync(Guid tableId);
    Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status);
    Task<IReadOnlyList<Order>> GetActiveOrdersAsync();
    Task<Order?> GetOrderWithDetailsAsync(Guid orderId);
    Task<IReadOnlyList<Order>> GetUnpaidOrdersByTableAsync(Guid tableId);
    Task<decimal> GetTotalAmountByTableAsync(Guid tableId);
}







