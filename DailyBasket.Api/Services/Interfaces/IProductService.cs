using DailyBasket.Api.DTOs.Products;

namespace DailyBasket.Api.Services.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(int? categoryId = null);
    Task<ProductResponse> GetByIdAsync(int id);
    Task<ProductResponse> CreateAsync(ProductCreateRequest request);
    Task UpdateAsync(int id, ProductUpdateRequest request);
    Task DeleteAsync(int id);
}
