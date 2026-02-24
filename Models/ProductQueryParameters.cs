namespace MyFirstProject.Models;

public class ProductQueryParameters
{
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public int PageSize{get;set;} = 10;
    public int PageNumber{get;set;} = 1;
}