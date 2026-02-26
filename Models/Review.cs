using Microsoft.AspNetCore.Identity;

namespace MyFirstProject.Models;

public class Review
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public IdentityUser User { get; set; } = null!;
    public required int Rating{ get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; }
    public required int ProductId { get; set; }
    public Product Product{ get; set; } = null!;
}