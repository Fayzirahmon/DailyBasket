using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.DTOs.Cart;

public class UpdateCartItemRequest
{
    [Range(1, 999, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}
