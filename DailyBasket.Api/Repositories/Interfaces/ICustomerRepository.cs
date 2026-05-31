using DailyBasket.Api.Models;

namespace DailyBasket.Api.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer> AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Customer customer);
    Task<bool> HasCartItemsAsync(int id);
    Task<bool> HasOrdersAsync(int id);
}
