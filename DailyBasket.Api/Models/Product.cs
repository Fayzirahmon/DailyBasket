using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.Models;

public class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    [Required]
    [MaxLength(120)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    public Category? Category { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
