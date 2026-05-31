using DailyBasket.Api.DTOs.Categories;

namespace DailyBasket.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CategoryCreateRequest request);
    Task UpdateAsync(int id, CategoryUpdateRequest request);
    Task DeleteAsync(int id);
}
