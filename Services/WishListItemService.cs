using Microsoft.Extensions.Caching.Distributed;
using MyFirstProject.Constants;

namespace MyFirstProject.Services;

public class WishListItemService : IWishListItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _cache;

    public WishListItemService(IUnitOfWork unitOfWork, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<List<WishListItemDto>>> GetAllWishListItemAsync(string userId)
    {
        var wishListItems = await _unitOfWork.WishListItem.GetAllWishItemAsync(userId);

        var result = wishListItems.Select(p => new WishListItemDto
        {
            Id = p.Id,
            UserId = p.UserId,
            ProductId = p.ProductId,
            CreateAt = p.CreateAt
        }).ToList();

        return Result<List<WishListItemDto>>.Success(result);
    }

    public async Task<Result<WishListItemDto>> GetWishListItemById(string userId, int wishListItemId)
    {
        var wishListItem = await _unitOfWork.WishListItem.GetWishListItemAsync(userId, wishListItemId);

        if(wishListItem == null)
        {
            return Result<WishListItemDto>.Failure(DomainErrors.WishList.WishListNotFound);
        }

        var wishListItemDto = new WishListItemDto
        {
            Id = wishListItem.Id,
            UserId = wishListItem.UserId,
            ProductId = wishListItem.ProductId,
            CreateAt = wishListItem.CreateAt,
        };
        return Result<WishListItemDto>.Success(wishListItemDto);
    }

    public async Task<Result<WishListItemDto>> AddItemToWishListAsync(int productId, string userId)
    {
        int wishListCount = await _unitOfWork.WishListItem.GetUserWishListItemCountAsync(userId);

        if(wishListCount >= 500)
        {
            return Result<WishListItemDto>.Failure(DomainErrors.WishList.LimitReached);
        }

        var product = await _unitOfWork.Product.GetByIdProduct(productId);

        if(product == null)
        {
            return Result<WishListItemDto>.Failure(DomainErrors.Product.ProductNotFound);
        }

        var newWishListItem = new WishListItem
        {
            UserId = userId,
            ProductId = productId,
            CreateAt = DateTime.UtcNow
        };

        await _unitOfWork.WishListItem.AddItemToWishListAsync(newWishListItem);
        await _unitOfWork.SaveChangesAsync();

        var wishListToReturn = new WishListItemDto{
            Id = newWishListItem.Id,
            UserId = newWishListItem.UserId,
            ProductId = newWishListItem.ProductId,
            CreateAt = newWishListItem.CreateAt
        };

        await _cache.RemoveAsync(CachedKeys.WishList(wishListToReturn.Id));

        return Result<WishListItemDto>.Success(wishListToReturn);
    }

    public async Task<Result<WishListItemDto>> DeleteItemFromWishListAsync(int id, string userId)
    {
        var itemOfWishList = await _unitOfWork.WishListItem.GetWishListItemAsync(userId, id);

        if (itemOfWishList == null)
        {
            return Result<WishListItemDto>.Failure(DomainErrors.WishList.ItemNotFound);
        }

        _unitOfWork.WishListItem.DeleteItemFromWishList(itemOfWishList);
        await _unitOfWork.SaveChangesAsync();

        return Result<WishListItemDto>.Success();
    }
}