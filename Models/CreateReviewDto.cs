namespace MyFirstProject.Models;

public class CreateReviewDto
{
    public required int Rating{ get; set; }
    public string Body { get; set; } = string.Empty;
}