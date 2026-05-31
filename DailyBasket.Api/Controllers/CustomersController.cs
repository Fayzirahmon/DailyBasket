// ====================================================================
// Component: Controllers/CustomersController.cs
// Layer: Presentation Layer (API Endpoints)
// Responsibility: Handles HTTP requests for customer account management.
// ====================================================================

using DailyBasket.Api.DTOs.Customers;
using DailyBasket.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DailyBasket.Api.Controllers;

/// <summary>
/// API controller providing endpoints for customer CRUD operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all registered customers.
    /// </summary>
    /// <returns>An ActionResult containing a read-only list of <see cref="CustomerResponse"/>.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> GetAll()
    {
        return Ok(await customerService.GetAllAsync());
    }

    /// <summary>
    /// Retrieves a specific customer by their unique database ID.
    /// </summary>
    /// <param name="id">The unique ID of the customer.</param>
    /// <returns>An ActionResult containing the <see cref="CustomerResponse"/>.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id)
    {
        return Ok(await customerService.GetByIdAsync(id));
    }

    /// <summary>
    /// Registers/creates a new customer in the system.
    /// </summary>
    /// <param name="request">The data payload containing details of the new customer.</param>
    /// <returns>An ActionResult containing the created <see cref="CustomerResponse"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CustomerCreateRequest request)
    {
        var customer = await customerService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, customer);
    }

    /// <summary>
    /// Updates the details of an existing customer.
    /// </summary>
    /// <param name="id">The unique ID of the customer to update.</param>
    /// <param name="request">The updated customer data payload.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CustomerUpdateRequest request)
    {
        await customerService.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a customer account from the system.
    /// </summary>
    /// <param name="id">The unique ID of the customer to delete.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await customerService.DeleteAsync(id);
        return NoContent();
    }
}
