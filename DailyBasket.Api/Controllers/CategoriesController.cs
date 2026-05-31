// ====================================================================
// Component: Controllers/CategoriesController.cs
// Layer: Presentation Layer (API Endpoints)
// Responsibility: Handles HTTP requests for grocery categories.
// ====================================================================

using DailyBasket.Api.DTOs.Categories;
using DailyBasket.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DailyBasket.Api.Controllers;

/// <summary>
/// API controller providing endpoints for category CRUD operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all product categories.
    /// </summary>
    /// <returns>An ActionResult containing a read-only list of <see cref="CategoryResponse"/>.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll()
    {
        return Ok(await categoryService.GetAllAsync());
    }

    /// <summary>
    /// Retrieves a specific product category by its unique database ID.
    /// </summary>
    /// <param name="id">The unique ID of the category.</param>
    /// <returns>An ActionResult containing the <see cref="CategoryResponse"/>.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        return Ok(await categoryService.GetByIdAsync(id));
    }

    /// <summary>
    /// Creates a new product category.
    /// </summary>
    /// <param name="request">The data payload containing details of the new category.</param>
    /// <returns>An ActionResult containing the created <see cref="CategoryResponse"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryCreateRequest request)
    {
        var category = await categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
    }

    /// <summary>
    /// Updates the details of an existing product category.
    /// </summary>
    /// <param name="id">The unique ID of the category to update.</param>
    /// <param name="request">The updated category data payload.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryUpdateRequest request)
    {
        await categoryService.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a product category from the system.
    /// </summary>
    /// <param name="id">The unique ID of the category to delete.</param>
    /// <returns>An empty 204 NoContent result on success.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await categoryService.DeleteAsync(id);
        return NoContent();
    }
}
