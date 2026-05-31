using DailyBasket.Api.Data;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DailyBasket.Api.Repositories;

public class CategoryRepository(DailyBasketDbContext dbContext) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetAllAsync()
    {
        return await dbContext.Categories
            .Include(category => category.Products)
            .AsNoTracking()
            .OrderBy(category => category.CategoryName)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await dbContext.Categories
            .Include(category => category.Products)
            .FirstOrDefaultAsync(category => category.CategoryId == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await dbContext.Categories.AnyAsync(category => category.CategoryId == id);
    }

    public async Task<bool> HasProductsAsync(int id)
    {
        return await dbContext.Products.AnyAsync(product => product.CategoryId == id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();
    }
}
