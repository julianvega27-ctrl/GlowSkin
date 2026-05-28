using AlaiaStore.Domain.Entities;

namespace AlaiaStore.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
}
