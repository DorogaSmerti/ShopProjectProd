using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface ICartService
{
    Task<List<CartItemDto>> GetCartAsync(string userId);
    Task<(CartResult cartResult, CartItemDto? cartItemDto)> AddToCartAsync(string userId, int productId, int quantity);
    Task<CartResult> DeleteFromCartAsync(string userId, int cartItemId);
    Task<CartResult> UpdateQuantityAsync(string userId, int cartItemId, int quantity);
}