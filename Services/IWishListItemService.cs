namespace MyFirstProject.Services;
public interface IWishListItemService
{
    Task<Result<List<WishListItemDto>>> GetAllWishListItemAsync(string userId);
    Task<Result<WishListItemDto>> GetWishListItemById(string userId, int wishListItemId);
    Task<Result<WishListItemDto>> AddItemToWishListAsync(int productId, string userId);
    Task<Result<WishListItemDto>> DeleteItemFromWishListAsync(int itemOfWishList, string userId);
}