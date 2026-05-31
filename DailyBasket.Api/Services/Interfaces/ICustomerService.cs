using DailyBasket.Api.DTOs.Customers;

namespace DailyBasket.Api.Services.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponse>> GetAllAsync();
    Task<CustomerResponse> GetByIdAsync(int id);
    Task<CustomerResponse> CreateAsync(CustomerCreateRequest request);
    Task UpdateAsync(int id, CustomerUpdateRequest request);
    Task DeleteAsync(int id);
}
