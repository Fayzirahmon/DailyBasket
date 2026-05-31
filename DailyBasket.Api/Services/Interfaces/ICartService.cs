using DailyBasket.Api.DTOs.Cart;

namespace DailyBasket.Api.Services.Interfaces;

public interface ICartService
{
    Task<CartSummaryResponse> GetByCustomerAsync(int customerId);
    Task<CartItemResponse> AddAsync(AddCartItemRequest request);
    Task<CartItemResponse> UpdateAsync(int cartItemId, UpdateCartItemRequest request);
    Task DeleteAsync(int cartItemId);
    Task ClearAsync(int customerId);
}
