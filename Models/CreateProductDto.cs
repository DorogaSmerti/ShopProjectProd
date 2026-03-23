namespace MyFirstProject.Models;

public record CreateProductDto
{
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public required decimal Price { get; init; }
    public required int Stock { get; init; }

}