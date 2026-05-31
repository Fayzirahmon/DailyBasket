using DailyBasket.Api.Models;

namespace DailyBasket.Api.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync();
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(int customerId);
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetTrackedByIdAsync(int id);
    Task UpdateAsync(Order order);
    Task<Order> CreateFromCartAsync(Customer customer, IReadOnlyList<CartItem> cartItems, string deliveryAddress);
}
