using DailyBasket.Api.Data;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DailyBasket.Api.Repositories;

public class CustomerRepository(DailyBasketDbContext dbContext) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync()
    {
        return await dbContext.Customers
            .Include(customer => customer.CartItems)
            .Include(customer => customer.Orders)
            .AsSplitQuery()
            .AsNoTracking()
            .OrderBy(customer => customer.FullName)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await dbContext.Customers
            .Include(customer => customer.CartItems)
            .Include(customer => customer.Orders)
            .AsSplitQuery()
            .FirstOrDefaultAsync(customer => customer.CustomerId == id);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await dbContext.Customers
            .FirstOrDefaultAsync(customer => customer.Email == email);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Customer customer)
    {
        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> HasCartItemsAsync(int id)
    {
        return await dbContext.CartItems.AnyAsync(cartItem => cartItem.CustomerId == id);
    }

    public async Task<bool> HasOrdersAsync(int id)
    {
        return await dbContext.Orders.AnyAsync(order => order.CustomerId == id);
    }
}
