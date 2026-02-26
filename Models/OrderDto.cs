namespace MyFirstProject.Models;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public required OrderStatus Status { get; set; }
    public required decimal TotalAmount { get; set; }
    public required string UserId { get; set; }
    public required List<OrderItemDto> OrderItems { get; set; }
}