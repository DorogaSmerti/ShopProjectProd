using Microsoft.AspNetCore.Identity;
using MyFirstProject.Models;

public class WishListItem
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public IdentityUser User{ get; set; } = null!;
    public required int ProductId { get; set; }
    public Product Product{ get; set; } = null!;
    public required DateTime CreateAt{ get; set; }
}