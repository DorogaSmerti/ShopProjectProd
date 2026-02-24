namespace MyFirstProject.Services;
public interface IWishListItemService
{
    Task<List<WishListItemDto>> GetAllWishListItemAsync(string userId);
    Task<Result<WishListItemDto>> AddItemToWishListAsync(int productId, string userId);
    Task<Result<WishListItemDto>> DeleteItemFromWishListAsync(int itemOfWishList, string userId);
}