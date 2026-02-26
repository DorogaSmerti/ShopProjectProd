namespace MyFirstProject.Models;

public class ReviewsDto
{
    public int Id{ get; set; }
    public required string Username { get; set; }
    public required int Rating{ get; set; }
    public string Body { get; set; } = string.Empty;
    public int ProductId{ get; set; }
    public DateTime CreatedAt { get; set; }
}