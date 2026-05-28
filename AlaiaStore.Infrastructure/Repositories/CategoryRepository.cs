using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Infrastructure.Data;

namespace AlaiaStore.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}