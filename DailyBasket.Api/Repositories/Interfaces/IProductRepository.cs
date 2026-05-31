using DailyBasket.Api.Models;

namespace DailyBasket.Api.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(int? categoryId = null);
    Task<Product?> GetByIdAsync(int id);
    Task<bool> HasCartItemsAsync(int id);
    Task<bool> HasOrderItemsAsync(int id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
