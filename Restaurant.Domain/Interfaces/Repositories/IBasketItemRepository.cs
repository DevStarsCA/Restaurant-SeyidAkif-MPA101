using Restaurant.Domain.Entities;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface IBasketItemRepository : IGenericRepository<BasketItem>
{
    Task<IReadOnlyList<BasketItem>> GetBasketItemsByTableIdAsync(Guid tableId);
    Task ClearBasketByTableIdAsync(Guid tableId);
}







