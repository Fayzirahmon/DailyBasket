using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.DTOs.Orders;

public class UpdateOrderStatusRequest
{
    [Required]
    [MaxLength(40)]
    public string Status { get; set; } = string.Empty;
}
