using Microsoft.EntityFrameworkCore;
using MyFirstProject.Data;
using MyFirstProject.Models;

namespace MyFirstProject.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CartItem>> GetCartAsync(string userId)
    {
        return await _context.CartItems
            .Include(p => p.Product)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<CartItem> GetCartItemAsync(string userId, int cartItemId)
    {
        return await _context.CartItems
            .Include(p => p.Product)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.CartItemId == cartItemId);
    }

    public async Task<CartItem> GetCartItemByProductAsync(string userId, int productId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(p => p.UserId == userId && p.ProductId == productId);
    }

    public async Task AddToCartAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    public void DeleteFromCart(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
    }

    public void RemoveRangeFromCart(List<CartItem> cartItems)
    {
        _context.CartItems.RemoveRange(cartItems);
    }

    public void UpdateQuantity(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
    }
}