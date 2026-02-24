using MyFirstProject.Models;

namespace MyFirstProject.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CartItemDto>> GetCartAsync(string userId)
    {
        var cartItem = await _unitOfWork.CartItem.GetCartAsync(userId);

        var cartDto = cartItem.Select(item => new CartItemDto
        {
            Id = item.CartItemId,
            Price = item.Product.Price,
            CartItemQuantity = item.QuantityCartItem,
            UserId = item.UserId,
            ProductId = item.ProductId
        }).ToList();
        
        return cartDto;
    }

    public async Task<(CartResult cartResult, CartItemDto? cartItemDto)> AddToCartAsync(string userId, int productId, int quantity)
    {
        var product = await _unitOfWork.Products.GetByIdProduct(productId);

        if (product == null)
        {
            return(CartResult.ProductNotFound, null);
        }

        if (product.Stock < quantity)
        {
            return (CartResult.NotEnoughStock, null);
        }

        var existingItem = await _unitOfWork.CartItem.GetCartItemByProductAsync(userId, productId);

        CartItem itemToReturn;

        if (existingItem != null)
        {
            existingItem.QuantityCartItem += quantity;
            itemToReturn = existingItem;
        }
        else
        {
            var newCartItem = new CartItem
            {
                QuantityCartItem = quantity,
                UserId = userId,
                ProductId = product.Id,
            };
            await _unitOfWork.CartItem.AddToCartAsync(newCartItem);
            itemToReturn = newCartItem;
        }

        await _unitOfWork.CompleteAsync();

        var resultDto = new CartItemDto
        {
            Id = itemToReturn.CartItemId,
            Price = product.Price,
            CartItemQuantity = itemToReturn.QuantityCartItem,
            UserId = itemToReturn.UserId,
            ProductId = itemToReturn.ProductId
        };
        return (CartResult.Success, resultDto);
    }

    public async Task<CartResult> DeleteFromCartAsync(string userId, int cartItemId)
    {
        var cartItem = await _unitOfWork.CartItem.GetCartItemAsync(userId, cartItemId);

        if (cartItem == null)
        {
            return CartResult.ItemNotFound;
        }

        _unitOfWork.CartItem.DeleteFromCart(cartItem);
        await _unitOfWork.CompleteAsync();
        return CartResult.Success;
    }
    
    public async Task<CartResult> UpdateQuantityAsync(string userId, int cartItemId, int quantity)
    {
        var cartItem = await _unitOfWork.CartItem.GetCartItemAsync(userId, cartItemId);

        if (cartItem == null)
        {
            return CartResult.ItemNotFound;
        }

        var product = await _unitOfWork.Products.GetByIdProduct(cartItem.ProductId);

        if (product == null)
        {
            return CartResult.ProductNotFound;
        }

        if (product.Stock < quantity)
        {
            return CartResult.NotEnoughStock;
        }

        if (quantity == 0)
        {
            _unitOfWork.CartItem.DeleteFromCart(cartItem);
        }
        else
        {
            cartItem.QuantityCartItem = quantity;
        }
        await _unitOfWork.CompleteAsync();
        return CartResult.Success;
    }
}