namespace DailyBasket.Api.Models;

public class CartItem
{
    public int CartItemId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public Customer? Customer { get; set; }

    public Product? Product { get; set; }
}
