using DailyBasket.Api.Data;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DailyBasket.Api.Repositories;

public class ProductRepository(DailyBasketDbContext dbContext) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(int? categoryId = null)
    {
        var query = dbContext.Products
            .Include(product => product.Category)
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(product => product.ProductName)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await dbContext.Products
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.ProductId == id);
    }

    public async Task<bool> HasCartItemsAsync(int id)
    {
        return await dbContext.CartItems.AnyAsync(cartItem => cartItem.ProductId == id);
    }

    public async Task<bool> HasOrderItemsAsync(int id)
    {
        return await dbContext.OrderItems.AnyAsync(orderItem => orderItem.ProductId == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }
}
