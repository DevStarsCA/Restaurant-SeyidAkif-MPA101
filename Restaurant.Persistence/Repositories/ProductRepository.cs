using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Where(x => x.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetAvailableProductsAsync()
    {
        return await _dbSet
            .Include(x => x.Category)
            .Where(x => x.IsAvailable)
            .AsNoTracking()
            .ToListAsync();
    }
}