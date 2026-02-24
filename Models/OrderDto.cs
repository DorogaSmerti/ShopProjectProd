namespace MyFirstProject.Models;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string UserId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}