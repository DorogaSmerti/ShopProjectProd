namespace MyFirstProject.Models;

public class CartItem
{
    public int CartItemId { get; set; }
    public int QuantityCartItem { get; set; }
    public required string UserId { get; set; }
    public int ProductId { get; set; }
    public Product Product{ get; set; } = null!;
}