using MyFirstProject.Models;

namespace MyFirstProject.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<CartItemDto>>> GetCartAsync(string userId)
    {
        var cartItem = await _unitOfWork.CartItem.GetCartAsync(userId);

        if (cartItem == null || !cartItem.Any())
        {
            return Result<List<CartItemDto>>.Failure(DomainErrors.Cart.CartNotFound);
        }

        var cartDto = cartItem.Select(item => new CartItemDto
        {
            Id = item.CartItemId,
            Price = item.Product.Price,
            CartItemQuantity = item.QuantityCartItem,
            UserId = item.UserId,
            ProductId = item.ProductId
        }).ToList();
        
        return Result<List<CartItemDto>>.Success(cartDto);
    }

    public async Task<Result<CartItemDto>> AddToCartAsync(string userId, int productId, int quantity)
    {
        var product = await _unitOfWork.Products.GetByIdProduct(productId);

        if (product == null)
        {
            return Result<CartItemDto>.Failure(DomainErrors.Cart.ProductNotFound);
        }

        if (product.Stock < quantity)
        {
            return Result<CartItemDto>.Failure(DomainErrors.Cart.NotEnoughStock);
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
        return Result<CartItemDto>.Success(resultDto);
    }

    public async Task<Result<bool>> DeleteFromCartAsync(string userId, int cartItemId)
    {
        var cartItem = await _unitOfWork.CartItem.GetCartItemAsync(userId, cartItemId);

        if (cartItem == null)
        {
            return Result<bool>.Failure(DomainErrors.Cart.CartNotFound);
        }

        _unitOfWork.CartItem.DeleteFromCart(cartItem);
        await _unitOfWork.CompleteAsync();
        return Result<bool>.Success(true);
    }
    
    public async Task<Result<bool>> UpdateQuantityAsync(string userId, int cartItemId, int quantity)
    {
        var cartItem = await _unitOfWork.CartItem.GetCartItemAsync(userId, cartItemId);

        if (cartItem == null)
        {
            return Result<bool>.Failure(DomainErrors.Cart.CartNotFound);
        }

        var product = await _unitOfWork.Products.GetByIdProduct(cartItem.ProductId);

        if (product == null)
        {
            return Result<bool>.Failure(DomainErrors.Cart.ProductNotFound);
        }

        if (product.Stock < quantity)
        {
            return Result<bool>.Failure(DomainErrors.Cart.NotEnoughStock);
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
        return Result<bool>.Success(true);
    }
}