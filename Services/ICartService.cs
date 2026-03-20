using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface ICartService
{
    Task<Result<List<CartItemDto>>> GetCartAsync(string userId);
    Task<Result<CartItemDto>> AddToCartAsync(string userId, int productId, int quantity);
    Task<Result<CartItemDto>> DeleteFromCartAsync(string userId, int cartItemId);
    Task<Result<CartItemDto>> UpdateQuantityAsync(string userId, int cartItemId, int quantity);
}