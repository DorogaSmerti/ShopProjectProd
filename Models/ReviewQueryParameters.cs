namespace MyFirstProject.Models;

public class ReviewQueryParameters
{
    public int? MaxRating{get;set;}
    public int? MinRatiog{get;set;}

    public int PageSize{get;set;} = 10;
    public int PageNumber{get;set;} = 1;
}