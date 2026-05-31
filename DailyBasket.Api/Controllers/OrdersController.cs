// ====================================================================
// Component: Controllers/OrdersController.cs
// Layer: Presentation Layer (API Endpoints)
// Responsibility: Handles HTTP requests for customer order operations.
// ====================================================================

using DailyBasket.Api.DTOs.Orders;
using DailyBasket.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DailyBasket.Api.Controllers;

/// <summary>
/// API controller providing endpoints for order queries, status changes, and checkouts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all orders registered in the system (Admin only view).
    /// </summary>
    /// <returns>An ActionResult containing a read-only list of <see cref="OrderResponse"/>.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAll()
    {
        return Ok(await orderService.GetAllAsync());
    }

    /// <summary>
    /// Retrieves a specific order by its unique database ID.
    /// </summary>
    /// <param name="id">The unique ID of the order.</param>
    /// <returns>An ActionResult containing the <see cref="OrderResponse"/>.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(int id)
    {
        return Ok(await orderService.GetByIdAsync(id));
    }

    /// <summary>
    /// Retrieves all orders made by a specific customer.
    /// </summary>
    /// <param name="customerId">The unique ID of the customer.</param>
    /// <returns>An ActionResult containing a read-only list of <see cref="OrderResponse"/>.</returns>
    [HttpGet("customer/{customerId:int}")]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetByCustomer(int customerId)
    {
        return Ok(await orderService.GetByCustomerAsync(customerId));
    }

    /// <summary>
    /// Places an order by checking out the items in a customer's active shopping cart.
    /// </summary>
    /// <param name="customerId">The unique ID of the checking-out customer.</param>
    /// <param name="request">The checkout request payload (e.g., delivery address).</param>
    /// <returns>An ActionResult containing the created <see cref="OrderResponse"/>.</returns>
    [HttpPost("checkout/{customerId:int}")]
    public async Task<ActionResult<OrderResponse>> Checkout(int customerId, CheckoutRequest request)
    {
        var order = await orderService.CheckoutAsync(customerId, request);
        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
    }

    /// <summary>
    /// Updates the delivery status of an existing order (e.g., Pending, Processing, Shipped, Delivered).
    /// </summary>
    /// <param name="id">The unique ID of the order.</param>
    /// <param name="request">The payload containing the new order status.</param>
    /// <returns>An ActionResult containing the updated <see cref="OrderResponse"/>.</returns>
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        return Ok(await orderService.UpdateStatusAsync(id, request));
    }
}
