using Microsoft.AspNetCore.Identity;

namespace MyFirstProject.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public required OrderStatus Status { get; set; }
    public required decimal TotalAmount { get; set; }
    public required string UserId{ get; set; }
    public IdentityUser IdentityUser { get; set; } = null!;
    public required List<OrderItem> OrderItems{ get; set; }
}