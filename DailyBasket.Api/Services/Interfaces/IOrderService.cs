using DailyBasket.Api.DTOs.Orders;

namespace DailyBasket.Api.Services.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderResponse>> GetAllAsync();
    Task<IReadOnlyList<OrderResponse>> GetByCustomerAsync(int customerId);
    Task<OrderResponse> GetByIdAsync(int id);
    Task<OrderResponse> CheckoutAsync(int customerId, CheckoutRequest request);
    Task<OrderResponse> UpdateStatusAsync(int id, UpdateOrderStatusRequest request);
}
