namespace MyFirstProject.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int Stock { get; set; }
    public List<Review> Reviews{ get; set; } = new();
    public DateTime CreateAt{ get; set; }
}