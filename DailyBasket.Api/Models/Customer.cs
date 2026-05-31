using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.Models;

public class Customer
{
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
