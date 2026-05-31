// ====================================================================
// Component: Services/CartService.cs
// Layer: Business Logic Layer
// Responsibility: Handles shopping cart validations, addition, updates, and clearing.
// ====================================================================

using DailyBasket.Api.DTOs.Cart;
using DailyBasket.Api.Exceptions;
using DailyBasket.Api.Mappings;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using DailyBasket.Api.Services.Interfaces;

namespace DailyBasket.Api.Services;

/// <summary>
/// Service implementation managing all business rules for shopping carts.
/// Integrates cart items, customer accounts, and product inventory checks.
/// </summary>
public class CartService(
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IProductRepository productRepository) : ICartService
{
    /// <summary>
    /// Retrieves the current shopping cart items and maps them to a summary response for a specific customer.
    /// </summary>
    /// <param name="customerId">The unique ID of the customer.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="CartSummaryResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the customer does not exist in the database.</exception>
    public async Task<CartSummaryResponse> GetByCustomerAsync(int customerId)
    {
        var customer = await customerRepository.GetByIdAsync(customerId)
            ?? throw new NotFoundException($"Customer with ID {customerId} was not found.");

        var cartItems = await cartRepository.GetByCustomerIdAsync(customerId);
        return customer.ToCartSummary(cartItems);
    }

    /// <summary>
    /// Adds a product to a customer's cart, or increments the quantity if it already exists.
    /// Validates product availability and stock limits.
    /// </summary>
    /// <param name="request">The payload containing customer ID, product ID, and quantity.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="CartItemResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the customer or product is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when the product is out of stock or inactive.</exception>
    public async Task<CartItemResponse> AddAsync(AddCartItemRequest request)
    {
        _ = await customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new NotFoundException($"Customer with ID {request.CustomerId} was not found.");

        var product = await productRepository.GetByIdAsync(request.ProductId)
            ?? throw new NotFoundException($"Product with ID {request.ProductId} was not found.");

        EnsureCanAddToCart(product, request.Quantity);

        var existingItem = await cartRepository.GetByCustomerAndProductAsync(request.CustomerId, request.ProductId);
        if (existingItem is not null)
        {
            var newQuantity = existingItem.Quantity + request.Quantity;
            EnsureCanAddToCart(product, newQuantity);
            existingItem.Quantity = newQuantity;
            await cartRepository.UpdateAsync(existingItem);
            return existingItem.ToResponse();
        }

        var cartItem = new CartItem
        {
            CustomerId = request.CustomerId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        await cartRepository.AddAsync(cartItem);
        return (await cartRepository.GetByIdAsync(cartItem.CartItemId))!.ToResponse();
    }

    /// <summary>
    /// Updates the quantity of a specific cart item and verifies stock availability.
    /// </summary>
    /// <param name="cartItemId">The unique database ID of the cart item.</param>
    /// <param name="request">The update payload containing the new target quantity.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="CartItemResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the cart item does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the requested quantity exceeds available stock.</exception>
    public async Task<CartItemResponse> UpdateAsync(int cartItemId, UpdateCartItemRequest request)
    {
        var cartItem = await cartRepository.GetByIdAsync(cartItemId)
            ?? throw new NotFoundException($"Cart item with ID {cartItemId} was not found.");

        EnsureCanAddToCart(cartItem.Product!, request.Quantity);
        cartItem.Quantity = request.Quantity;

        await cartRepository.UpdateAsync(cartItem);
        return cartItem.ToResponse();
    }

    /// <summary>
    /// Deletes a specific item completely from the cart.
    /// </summary>
    /// <param name="cartItemId">The unique database ID of the cart item to remove.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the cart item is not found.</exception>
    public async Task DeleteAsync(int cartItemId)
    {
        var cartItem = await cartRepository.GetByIdAsync(cartItemId)
            ?? throw new NotFoundException($"Cart item with ID {cartItemId} was not found.");

        await cartRepository.DeleteAsync(cartItem);
    }

    /// <summary>
    /// Clears all cart items for a specific customer.
    /// </summary>
    /// <param name="customerId">The unique ID of the customer whose cart should be cleared.</param>
    /// <returns>A task representing the asynchronous clear operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the customer is not found.</exception>
    public async Task ClearAsync(int customerId)
    {
        _ = await customerRepository.GetByIdAsync(customerId)
            ?? throw new NotFoundException($"Customer with ID {customerId} was not found.");

        await cartRepository.ClearCustomerCartAsync(customerId);
    }

    /// <summary>
    /// Core validation helper ensuring a product is available and has sufficient stock quantity.
    /// </summary>
    /// <param name="product">The product entity to validate.</param>
    /// <param name="quantity">The target purchase quantity.</param>
    /// <exception cref="BadRequestException">Thrown when product is unavailable or stock is insufficient.</exception>
    private static void EnsureCanAddToCart(Product product, int quantity)
    {
        if (!product.IsAvailable)
        {
            throw new BadRequestException("Product is currently unavailable.");
        }

        if (quantity > product.StockQuantity)
        {
            throw new BadRequestException($"Only {product.StockQuantity} units are available in stock.");
        }
    }
}
