using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.DTOs.Cart;

public class AddCartItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Customer is required.")]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
    public int ProductId { get; set; }

    [Range(1, 999, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}
