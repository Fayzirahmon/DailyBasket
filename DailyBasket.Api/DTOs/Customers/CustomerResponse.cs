namespace DailyBasket.Api.DTOs.Customers;

public class CustomerResponse
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int CartItemCount { get; set; }
    public int OrderCount { get; set; }
}
