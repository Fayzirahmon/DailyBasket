// ====================================================================
// Component: Controllers/ProductsController.cs
// Layer: Presentation Layer (API Endpoints)
// Responsibility: Handles HTTP requests for grocery product inventory.
// ====================================================================

using DailyBasket.Api.DTOs.Products;
using DailyBasket.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DailyBasket.Api.Controllers;

/// <summary>
/// API controller providing endpoints for managing products, including filtering by category.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// Retrieves all products, optionally filtered by a specific category.
    /// </summary>
    /// <param name="categoryId">Optional unique category database ID filter.</param>
    /// <returns>An ActionResult containing a read-only list of <see cref="ProductResponse"/>.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll([FromQuery] int? categoryId)
    {
        return Ok(await productService.GetAllAsync(categoryId));
    }

    /// <summary>
    /// Retrieves a specific product by its unique database ID.
    /// </summary>
    /// <param name="id">The unique database ID of the product.</param>
    /// <returns>An ActionResult containing the <see cref="ProductResponse"/>.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        return Ok(await productService.GetByIdAsync(id));
    }

    /// <summary>
    /// Creates a new product in the grocery inventory database.
    /// </summary>
    /// <param name="request">The data payload containing product details.</param>
    /// <returns>An ActionResult containing the created <see cref="ProductResponse"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(ProductCreateRequest request)
    {
        var product = await productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }

    /// <summary>
    /// Updates the details (price, stock quantity, metadata) of an existing product.
    /// </summary>
    /// <param name="id">The unique database ID of the product to update.</param>
    /// <param name="request">The updated product data payload.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductUpdateRequest request)
    {
        await productService.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a product from the system catalog.
    /// </summary>
    /// <param name="id">The unique database ID of the product to delete.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await productService.DeleteAsync(id);
        return NoContent();
    }
}
