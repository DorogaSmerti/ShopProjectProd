
public interface IWishListItemRepository
{
    Task<List<WishListItem>> GetAllWishItemAsync(string userId);
    Task<int> GetUserWishListItemCountAsync(string userId);
    Task<WishListItem> GetWishListItemAsync(string userId, int id);
    Task AddItemToWishListAsync(WishListItem wishListItem);
    void DeleteItemFromWishList(WishListItem wishListItem);
}