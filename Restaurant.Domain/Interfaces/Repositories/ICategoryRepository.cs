using Restaurant.Domain.Entities;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IReadOnlyList<Category>> GetCategoriesWithProductsAsync();
}







