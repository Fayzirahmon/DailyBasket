using DailyBasket.Api.Data;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DailyBasket.Api.Repositories;

public class OrderRepository(DailyBasketDbContext dbContext) : IOrderRepository
{
    public async Task<IReadOnlyList<Order>> GetAllAsync()
    {
        return await dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
            .AsSplitQuery()
            .AsNoTracking()
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(int customerId)
    {
        return await dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(order => order.CustomerId == customerId)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.OrderId == id);
    }

    public async Task<Order?> GetTrackedByIdAsync(int id)
    {
        return await dbContext.Orders.FirstOrDefaultAsync(order => order.OrderId == id);
    }

    public async Task UpdateAsync(Order order)
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<Order> CreateFromCartAsync(Customer customer, IReadOnlyList<CartItem> cartItems, string deliveryAddress)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var order = new Order
        {
            CustomerId = customer.CustomerId,
            OrderDate = DateTime.UtcNow,
            DeliveryAddress = deliveryAddress,
            Status = "Pending",
            TotalAmount = cartItems.Sum(item => item.Quantity * item.Product!.Price),
            OrderItems = cartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product!.Price
            }).ToList()
        };

        foreach (var cartItem in cartItems)
        {
            cartItem.Product!.StockQuantity -= cartItem.Quantity;
        }

        dbContext.Orders.Add(order);
        dbContext.CartItems.RemoveRange(cartItems);
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return (await GetByIdAsync(order.OrderId))!;
    }
}
