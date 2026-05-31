// ====================================================================
// Component: Controllers/CartController.cs
// Layer: Presentation Layer (API Endpoints)
// Responsibility: Handles HTTP requests for managing customer shopping carts.
// ====================================================================

using DailyBasket.Api.DTOs.Cart;
using DailyBasket.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DailyBasket.Api.Controllers;

/// <summary>
/// API controller providing endpoints for cart interactions including retrieving, adding, 
/// updating, deleting, and clearing shopping cart items.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    /// <summary>
    /// Retrieves the cart summary for a specific customer.
    /// </summary>
    /// <param name="customerId">The unique ID of the customer whose cart is being retrieved.</param>
    /// <returns>An ActionResult containing the <see cref="CartSummaryResponse"/>.</returns>
    [HttpGet("customer/{customerId:int}")]
    public async Task<ActionResult<CartSummaryResponse>> GetByCustomer(int customerId)
    {
        return Ok(await cartService.GetByCustomerAsync(customerId));
    }

    /// <summary>
    /// Adds a new product item or increments its quantity in a customer's shopping cart.
    /// </summary>
    /// <param name="request">The data payload containing customer ID, product ID, and quantity.</param>
    /// <returns>An ActionResult containing the newly created or updated <see cref="CartItemResponse"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<CartItemResponse>> Add(AddCartItemRequest request)
    {
        var item = await cartService.AddAsync(request);
        return CreatedAtAction(nameof(GetByCustomer), new { customerId = item.CustomerId }, item);
    }

    /// <summary>
    /// Updates the quantity of a specific item already present in the cart.
    /// </summary>
    /// <param name="cartItemId">The unique database ID of the cart item to update.</param>
    /// <param name="request">The payload containing the new target quantity.</param>
    /// <returns>An ActionResult containing the updated <see cref="CartItemResponse"/>.</returns>
    [HttpPut("{cartItemId:int}")]
    public async Task<ActionResult<CartItemResponse>> Update(int cartItemId, UpdateCartItemRequest request)
    {
        return Ok(await cartService.UpdateAsync(cartItemId, request));
    }

    /// <summary>
    /// Removes a specific line item completely from a customer's cart.
    /// </summary>
    /// <param name="cartItemId">The unique database ID of the cart item to remove.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpDelete("{cartItemId:int}")]
    public async Task<IActionResult> Delete(int cartItemId)
    {
        await cartService.DeleteAsync(cartItemId);
        return NoContent();
    }

    /// <summary>
    /// Clears all shopping cart items for a specific customer.
    /// </summary>
    /// <param name="customerId">The unique ID of the customer whose cart should be cleared.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpDelete("customer/{customerId:int}")]
    public async Task<IActionResult> Clear(int customerId)
    {
        await cartService.ClearAsync(customerId);
        return NoContent();
    }
}
