using Microsoft.EntityFrameworkCore;
using MyFirstProject.Data;

namespace MyFirstProject.Repositories;

public class WishListItemRepository : IWishListItemRepository
{
    private readonly AppDbContext _context;

    public WishListItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WishListItem>> GetAllWishItemAsync(string userId)
    {
        return await _context.WishListItems.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<int> GetUserWishListItemCountAsync(string userId)
    {
        return await _context.WishListItems.CountAsync(p => p.UserId == userId);
    }

    public async Task AddItemToWishListAsync(WishListItem wishListItem)
    {
        await _context.WishListItems.AddAsync(wishListItem);
    }

    public async Task<WishListItem> GetWishListItemAsync(string userId, int id)
    {
        return await _context.WishListItems.FirstOrDefaultAsync(p => userId == p.UserId && p.Id == id);
    }

    public void DeleteItemFromWishList(WishListItem wishListItem)
    {
        _context.WishListItems.Remove(wishListItem);
    }

}