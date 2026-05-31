using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(80)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
