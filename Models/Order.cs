using Microsoft.AspNetCore.Identity;

namespace MyFirstProject.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string UserId{ get; set; }
    public IdentityUser IdentityUser { get; set; }
    public List<OrderItem> OrderItems{ get; set; }
}