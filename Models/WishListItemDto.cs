public class WishListItemDto
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required int ProductId { get; set; }
    public required DateTime CreateAt{ get; set; }
}