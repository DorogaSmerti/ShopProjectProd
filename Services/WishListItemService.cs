using Microsoft.Extensions.Caching.Distributed;

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

    public async Task<Result<WishListItemDto>> AddItemToWishListAsync(int productId, string userId)
    {
        int wishListCount = await _unitOfWork.WishListItem.GetUserWishListItemCountAsync(userId);

        if(wishListCount >= 500)
        {
            return Result<WishListItemDto>.Failure(DomainErrors.WishList.LimitReached);
        }

        var newWishListItem = new WishListItem
        {
            UserId = userId,
            ProductId = productId,
            CreateAt = DateTime.UtcNow
        };

        await _unitOfWork.WishListItem.AddItemToWishListAsync(newWishListItem);
        await _unitOfWork.CompleteAsync();

        var wishListToReturn = new WishListItemDto{
            Id = newWishListItem.Id,
            UserId = newWishListItem.UserId,
            ProductId = newWishListItem.ProductId,
            CreateAt = newWishListItem.CreateAt
        };

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
        await _unitOfWork.CompleteAsync();

        var wishListToReturn = new WishListItemDto{
            Id = itemOfWishList.Id,
            UserId = itemOfWishList.UserId,
            ProductId = itemOfWishList.ProductId,
            CreateAt = itemOfWishList.CreateAt
        };

        return Result<WishListItemDto>.Success(wishListToReturn);
    }
}