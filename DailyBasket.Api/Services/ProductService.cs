// ====================================================================
// Component: Services/ProductService.cs
// Layer: Business Logic Layer
// Responsibility: Handles business logic, inventory checks, and validation for products.
// ====================================================================

using DailyBasket.Api.DTOs.Products;
using DailyBasket.Api.Exceptions;
using DailyBasket.Api.Mappings;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using DailyBasket.Api.Services.Interfaces;

namespace DailyBasket.Api.Services;

/// <summary>
/// Service implementation managing all business rules for grocery products,
/// including category validations and relational constraints blocking product deletion.
/// </summary>
public class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository) : IProductService
{
    /// <summary>
    /// Retrieves a list of all products in inventory, optionally filtered by a specific category.
    /// </summary>
    /// <param name="categoryId">Optional unique category ID to filter by.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of <see cref="ProductResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown if a category filter is provided but does not exist.</exception>
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(int? categoryId = null)
    {
        if (categoryId.HasValue && !await categoryRepository.ExistsAsync(categoryId.Value))
        {
            throw new NotFoundException($"Category with ID {categoryId.Value} was not found.");
        }

        var products = await productRepository.GetAllAsync(categoryId);
        return products.Select(product => product.ToResponse()).ToList();
    }

    /// <summary>
    /// Retrieves a specific product by its unique ID.
    /// </summary>
    /// <param name="id">The unique database product ID.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="ProductResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the product is not found.</exception>
    public async Task<ProductResponse> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with ID {id} was not found.");

        return product.ToResponse();
    }

    /// <summary>
    /// Registers a new product in the catalog, verifying the assigned category exists.
    /// </summary>
    /// <param name="request">The product creation payload.</param>
    /// <returns>A task representing the asynchronous operation, returning the created <see cref="ProductResponse"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when the assigned category does not exist.</exception>
    public async Task<ProductResponse> CreateAsync(ProductCreateRequest request)
    {
        if (!await categoryRepository.ExistsAsync(request.CategoryId))
        {
            throw new BadRequestException($"Category with ID {request.CategoryId} does not exist.");
        }

        var product = new Product
        {
            CategoryId = request.CategoryId,
            ProductName = request.ProductName.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ImageUrl = request.ImageUrl?.Trim(),
            IsAvailable = request.IsAvailable
        };

        await productRepository.AddAsync(product);
        return (await productRepository.GetByIdAsync(product.ProductId))!.ToResponse();
    }

    /// <summary>
    /// Updates an existing product's fields, validating the assigned category exists.
    /// </summary>
    /// <param name="id">The unique database ID of the product to update.</param>
    /// <param name="request">The product update payload.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the target product is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when the target category does not exist.</exception>
    public async Task UpdateAsync(int id, ProductUpdateRequest request)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with ID {id} was not found.");

        if (!await categoryRepository.ExistsAsync(request.CategoryId))
        {
            throw new BadRequestException($"Category with ID {request.CategoryId} does not exist.");
        }

        product.CategoryId = request.CategoryId;
        product.ProductName = request.ProductName.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.ImageUrl = request.ImageUrl?.Trim();
        product.IsAvailable = request.IsAvailable;

        await productRepository.UpdateAsync(product);
    }

    /// <summary>
    /// Deletes a product from the catalog, ensuring it does not appear in active customer carts or orders.
    /// </summary>
    /// <param name="id">The unique database ID of the product to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the product is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when attempting to delete a product linked to carts or orders.</exception>
    public async Task DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with ID {id} was not found.");

        if (await productRepository.HasCartItemsAsync(id) || await productRepository.HasOrderItemsAsync(id))
        {
            throw new BadRequestException("Product cannot be deleted while it appears in carts or orders.");
        }

        await productRepository.DeleteAsync(product);
    }
}
