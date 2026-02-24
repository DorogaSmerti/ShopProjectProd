using Microsoft.AspNetCore.Identity;

namespace MyFirstProject.Models;

public class Review
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public int Rating{ get; set; }
    public string Body { get; set; }
    public DateTime CreateAt { get; set; }
    public int ProductId { get; set; }
    public Product Product{ get; set; }
}