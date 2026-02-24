using Microsoft.AspNetCore.Identity;
using MyFirstProject.Models;

public class WishListItem
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public IdentityUser User{ get; set; }
    public int ProductId { get; set; }
    public Product Product{ get; set; }
    public DateTime CreateAt{ get; set; }
}