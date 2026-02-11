using Restaurant.Domain.Entities;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface IWaiterRepository : IGenericRepository<Waiter>
{
    Task<Waiter?> GetWaiterWithTablesAsync(Guid waiterId);
    Task<Waiter?> GetWaiterByUserIdAsync(string userId);
    Task<IReadOnlyList<Waiter>> GetActiveWaitersAsync();
}







