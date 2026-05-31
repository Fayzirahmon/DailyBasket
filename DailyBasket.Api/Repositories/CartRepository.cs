using DailyBasket.Api.Data;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DailyBasket.Api.Repositories;

public class CartRepository(DailyBasketDbContext dbContext) : ICartRepository
{
    public async Task<IReadOnlyList<CartItem>> GetByCustomerIdAsync(int customerId)
    {
        return await dbContext.CartItems
            .Include(cartItem => cartItem.Product)
            .Where(cartItem => cartItem.CustomerId == customerId)
            .OrderBy(cartItem => cartItem.Product!.ProductName)
            .ToListAsync();
    }

    public async Task<CartItem?> GetByIdAsync(int cartItemId)
    {
        return await dbContext.CartItems
            .Include(cartItem => cartItem.Product)
            .FirstOrDefaultAsync(cartItem => cartItem.CartItemId == cartItemId);
    }

    public async Task<CartItem?> GetByCustomerAndProductAsync(int customerId, int productId)
    {
        return await dbContext.CartItems
            .Include(cartItem => cartItem.Product)
            .FirstOrDefaultAsync(cartItem => cartItem.CustomerId == customerId && cartItem.ProductId == productId);
    }

    public async Task<CartItem> AddAsync(CartItem cartItem)
    {
        dbContext.CartItems.Add(cartItem);
        await dbContext.SaveChangesAsync();
        return cartItem;
    }

    public async Task UpdateAsync(CartItem cartItem)
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(CartItem cartItem)
    {
        dbContext.CartItems.Remove(cartItem);
        await dbContext.SaveChangesAsync();
    }

    public async Task ClearCustomerCartAsync(int customerId)
    {
        var items = await dbContext.CartItems
            .Where(cartItem => cartItem.CustomerId == customerId)
            .ToListAsync();

        dbContext.CartItems.RemoveRange(items);
        await dbContext.SaveChangesAsync();
    }
}
