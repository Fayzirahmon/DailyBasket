namespace DailyBasket.Api.DTOs.Cart;

public class CartItemResponse
{
    public int CartItemId { get; set; }
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int StockQuantity { get; set; }
    public decimal LineTotal { get; set; }
}
