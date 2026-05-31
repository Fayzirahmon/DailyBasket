using System.ComponentModel.DataAnnotations;

namespace DailyBasket.Api.DTOs.Categories;

public class CategoryCreateRequest
{
    [Required]
    [MaxLength(80)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }
}
