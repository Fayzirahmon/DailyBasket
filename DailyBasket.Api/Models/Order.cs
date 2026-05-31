using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.Models;

public class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(300)]
    public string DeliveryAddress { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(40)]
    public string Status { get; set; } = "Pending";

    public Customer? Customer { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
