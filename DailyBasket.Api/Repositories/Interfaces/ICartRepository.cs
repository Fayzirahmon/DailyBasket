using DailyBasket.Api.Models;

namespace DailyBasket.Api.Repositories.Interfaces;

public interface ICartRepository
{
    Task<IReadOnlyList<CartItem>> GetByCustomerIdAsync(int customerId);
    Task<CartItem?> GetByIdAsync(int cartItemId);
    Task<CartItem?> GetByCustomerAndProductAsync(int customerId, int productId);
    Task<CartItem> AddAsync(CartItem cartItem);
    Task UpdateAsync(CartItem cartItem);
    Task DeleteAsync(CartItem cartItem);
    Task ClearCustomerCartAsync(int customerId);
}
