using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.DTOs.Orders;

public class CheckoutRequest
{
    [Required]
    [MaxLength(300)]
    public string DeliveryAddress { get; set; } = string.Empty;
}
