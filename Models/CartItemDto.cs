namespace MyFirstProject.Models;

public class CartItemDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public int CartItemQuantity { get; set; }
    public required string UserId { get; set; }
    public int ProductId { get; set; }
}