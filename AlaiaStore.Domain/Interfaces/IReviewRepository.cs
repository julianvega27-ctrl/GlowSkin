using AlaiaStore.Domain.Entities;

namespace AlaiaStore.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
}
