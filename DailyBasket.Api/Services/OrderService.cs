// ====================================================================
// Component: Services/OrderService.cs
// Layer: Business Logic Layer
// Responsibility: Handles business rules for checking out and managing order statuses.
// ====================================================================

using DailyBasket.Api.DTOs.Orders;
using DailyBasket.Api.Exceptions;
using DailyBasket.Api.Mappings;
using DailyBasket.Api.Repositories.Interfaces;
using DailyBasket.Api.Services.Interfaces;

namespace DailyBasket.Api.Services;

/// <summary>
/// Service implementation managing all order workflows, transaction validity,
/// stock check rules, and status transition paths (e.g., Pending, Processing, etc.).
/// </summary>
public class OrderService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ICartRepository cartRepository) : IOrderService
{
    /// <summary>
    /// Supported list of valid order lifecycle statuses.
    /// </summary>
    private static readonly string[] AllowedStatuses = ["Pending", "Processing", "Delivered", "Cancelled"];

    /// <summary>
    /// Retrieves a list of all orders recorded in the system.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of <see cref="OrderResponse"/>.</returns>
    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync()
    {
        var orders = await orderRepository.GetAllAsync();
        return orders.Select(order => order.ToResponse()).ToList();
    }

    /// <summary>
    /// Retrieves all orders submitted by a specific customer.
    /// </summary>
    /// <param name="customerId">The unique customer ID.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of <see cref="OrderResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown if the customer does not exist.</exception>
    public async Task<IReadOnlyList<OrderResponse>> GetByCustomerAsync(int customerId)
    {
        _ = await customerRepository.GetByIdAsync(customerId)
            ?? throw new NotFoundException($"Customer with ID {customerId} was not found.");

        var orders = await orderRepository.GetByCustomerIdAsync(customerId);
        return orders.Select(order => order.ToResponse()).ToList();
    }

    /// <summary>
    /// Retrieves a specific order's summary by its unique ID.
    /// </summary>
    /// <param name="id">The unique database order ID.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="OrderResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown if the order is not found.</exception>
    public async Task<OrderResponse> GetByIdAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Order with ID {id} was not found.");

        return order.ToResponse();
    }

    /// <summary>
    /// Validates customer eligibility and active stock before completing checkout from the customer's cart.
    /// Triggers inventory adjustments and clears the cart on success.
    /// </summary>
    /// <param name="customerId">The unique database customer ID.</param>
    /// <param name="request">The checkout details (e.g., delivery address).</param>
    /// <returns>A task representing the asynchronous operation, returning the created <see cref="OrderResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown if the customer is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when the cart is empty, products are missing/unavailable, or quantity exceeds stock.</exception>
    public async Task<OrderResponse> CheckoutAsync(int customerId, CheckoutRequest request)
    {
        var customer = await customerRepository.GetByIdAsync(customerId)
            ?? throw new NotFoundException($"Customer with ID {customerId} was not found.");

        var cartItems = await cartRepository.GetByCustomerIdAsync(customerId);
        if (cartItems.Count == 0)
        {
            throw new BadRequestException("Cannot checkout because the cart is empty.");
        }

        foreach (var cartItem in cartItems)
        {
            var product = cartItem.Product;
            if (product is null)
            {
                throw new BadRequestException("Cart contains an invalid product.");
            }

            if (!product.IsAvailable)
            {
                throw new BadRequestException($"{product.ProductName} is currently unavailable.");
            }

            if (cartItem.Quantity > product.StockQuantity)
            {
                throw new BadRequestException($"{product.ProductName} only has {product.StockQuantity} units in stock.");
            }
        }

        var order = await orderRepository.CreateFromCartAsync(customer, cartItems, request.DeliveryAddress.Trim());
        return order.ToResponse();
    }

    /// <summary>
    /// Validates and updates the current lifecycle status of an existing order.
    /// </summary>
    /// <param name="id">The unique database order ID to update.</param>
    /// <param name="request">The update payload containing the target status.</param>
    /// <returns>A task representing the asynchronous operation, returning the updated <see cref="OrderResponse"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when the status value is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the order is not found.</exception>
    public async Task<OrderResponse> UpdateStatusAsync(int id, UpdateOrderStatusRequest request)
    {
        var normalizedStatus = AllowedStatuses.FirstOrDefault(
            status => string.Equals(status, request.Status.Trim(), StringComparison.OrdinalIgnoreCase));

        if (normalizedStatus is null)
        {
            throw new BadRequestException("Order status must be Pending, Processing, Delivered, or Cancelled.");
        }

        var order = await orderRepository.GetTrackedByIdAsync(id)
            ?? throw new NotFoundException($"Order with ID {id} was not found.");

        order.Status = normalizedStatus;
        await orderRepository.UpdateAsync(order);

        return await GetByIdAsync(id);
    }
}
