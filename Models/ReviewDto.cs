namespace MyFirstProject.Models;

public class ReviewsDto
{
    public int Id{ get; set; }
    public string Username { get; set; }
    public int Rating{ get; set; }
    public string Body { get; set; }
    public DateTime CreateAt { get; set; }
    public int productId{ get; set; }
}