using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Category>> GetCategoriesWithProductsAsync()
    {
        return await _dbSet
            .Include(x => x.Products.Where(p => !p.IsDeleted))
            .OrderBy(x => x.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }
}
