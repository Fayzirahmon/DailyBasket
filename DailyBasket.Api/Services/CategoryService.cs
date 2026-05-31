// ====================================================================
// Component: Services/CategoryService.cs
// Layer: Business Logic Layer
// Responsibility: Handles business logic and validations for product categories.
// ====================================================================

using DailyBasket.Api.DTOs.Categories;
using DailyBasket.Api.Exceptions;
using DailyBasket.Api.Mappings;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using DailyBasket.Api.Services.Interfaces;

namespace DailyBasket.Api.Services;

/// <summary>
/// Service implementation managing all business rules for product categories,
/// including relational constraint validations (preventing deletion of active categories).
/// </summary>
public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    /// <summary>
    /// Retrieves a list of all categories in the system.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of <see cref="CategoryResponse"/>.</returns>
    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        return categories.Select(category => category.ToResponse()).ToList();
    }

    /// <summary>
    /// Retrieves a specific category by its unique ID.
    /// </summary>
    /// <param name="id">The unique category ID.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="CategoryResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown if the category ID is not found.</exception>
    public async Task<CategoryResponse> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with ID {id} was not found.");

        return category.ToResponse();
    }

    /// <summary>
    /// Creates a new product category after trimming user text inputs.
    /// </summary>
    /// <param name="request">The category creation request payload.</param>
    /// <returns>A task representing the asynchronous operation, returning the created <see cref="CategoryResponse"/>.</returns>
    public async Task<CategoryResponse> CreateAsync(CategoryCreateRequest request)
    {
        var category = new Category
        {
            CategoryName = request.CategoryName.Trim(),
            Description = request.Description?.Trim()
        };

        await categoryRepository.AddAsync(category);
        return category.ToResponse();
    }

    /// <summary>
    /// Updates an existing category's properties.
    /// </summary>
    /// <param name="id">The unique database ID of the category to update.</param>
    /// <param name="request">The category update payload.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    /// <exception cref="NotFoundException">Thrown if the category is not found.</exception>
    public async Task UpdateAsync(int id, CategoryUpdateRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with ID {id} was not found.");

        category.CategoryName = request.CategoryName.Trim();
        category.Description = request.Description?.Trim();

        await categoryRepository.UpdateAsync(category);
    }

    /// <summary>
    /// Deletes a category if there are no products assigned to it.
    /// </summary>
    /// <param name="id">The unique database ID of the category to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the category is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when attempting to delete a category that still contains active products.</exception>
    public async Task DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with ID {id} was not found.");

        if (await categoryRepository.HasProductsAsync(id))
        {
            throw new BadRequestException("Category cannot be deleted while products are assigned to it.");
        }

        await categoryRepository.DeleteAsync(category);
    }
}
