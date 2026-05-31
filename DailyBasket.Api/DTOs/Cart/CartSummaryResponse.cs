namespace DailyBasket.Api.DTOs.Cart;

public class CartSummaryResponse
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<CartItemResponse> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
}
