using DailyBasket.Api.DTOs.Cart;
using DailyBasket.Api.DTOs.Categories;
using DailyBasket.Api.DTOs.Customers;
using DailyBasket.Api.DTOs.Orders;
using DailyBasket.Api.DTOs.Products;
using DailyBasket.Api.Models;

namespace DailyBasket.Api.Mappings;

public static class EntityMappings
{
    public static CategoryResponse ToResponse(this Category category)
    {
        return new CategoryResponse
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            ProductCount = category.Products.Count
        };
    }

    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse
        {
            ProductId = product.ProductId,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.CategoryName ?? string.Empty,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable
        };
    }

    public static CustomerResponse ToResponse(this Customer customer)
    {
        return new CustomerResponse
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            CartItemCount = customer.CartItems.Count,
            OrderCount = customer.Orders.Count
        };
    }

    public static CartItemResponse ToResponse(this CartItem cartItem)
    {
        var product = cartItem.Product;
        var unitPrice = product?.Price ?? 0m;

        return new CartItemResponse
        {
            CartItemId = cartItem.CartItemId,
            CustomerId = cartItem.CustomerId,
            ProductId = cartItem.ProductId,
            ProductName = product?.ProductName ?? string.Empty,
            ImageUrl = product?.ImageUrl,
            UnitPrice = unitPrice,
            Quantity = cartItem.Quantity,
            StockQuantity = product?.StockQuantity ?? 0,
            LineTotal = unitPrice * cartItem.Quantity
        };
    }

    public static CartSummaryResponse ToCartSummary(this Customer customer, IReadOnlyList<CartItem> cartItems)
    {
        var items = cartItems.Select(item => item.ToResponse()).ToList();

        return new CartSummaryResponse
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.FullName,
            Items = items,
            TotalItems = items.Sum(item => item.Quantity),
            TotalAmount = items.Sum(item => item.LineTotal)
        };
    }

    public static OrderResponse ToResponse(this Order order)
    {
        var items = order.OrderItems.Select(item => item.ToResponse()).ToList();

        return new OrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.FullName ?? string.Empty,
            OrderDate = order.OrderDate,
            DeliveryAddress = order.DeliveryAddress,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = items
        };
    }

    public static OrderItemResponse ToResponse(this OrderItem orderItem)
    {
        return new OrderItemResponse
        {
            OrderItemId = orderItem.OrderItemId,
            ProductId = orderItem.ProductId,
            ProductName = orderItem.Product?.ProductName ?? string.Empty,
            ImageUrl = orderItem.Product?.ImageUrl,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            LineTotal = orderItem.UnitPrice * orderItem.Quantity
        };
    }
}
