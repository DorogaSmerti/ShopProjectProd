using MyFirstProject.Models;

namespace MyFirstProject.Repositories;

public interface ICartRepository
{
    Task<List<CartItem>> GetCartAsync(string userId);
    Task<CartItem> GetCartItemAsync(string userId, int cartItemId);
    Task<CartItem> GetCartItemByProductAsync(string userId, int productId);
    Task AddToCartAsync(CartItem cartItem);
    void DeleteFromCart(CartItem cartItem);

    void RemoveRangeFromCart(List<CartItem> cartItems);
    void UpdateQuantity(CartItem cartItem);
}