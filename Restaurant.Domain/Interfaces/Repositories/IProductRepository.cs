using Restaurant.Domain.Entities;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId);
    Task<IReadOnlyList<Product>> GetAvailableProductsAsync();
}







