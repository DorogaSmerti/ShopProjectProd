namespace MyFirstProject.Models;

public class AddToCartDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class ChangeQuantityDto
{
    public int Quantity{ get; set; }
}