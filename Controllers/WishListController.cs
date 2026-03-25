using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;
using System.Security.Claims;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class WishListController : ApiControllerBase
{
    private readonly IWishListItemService _wishListItem;

    public WishListController(IWishListItemService wishListItem)
    {
        _wishListItem = wishListItem;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWishListItemAsync()
    {
        var result = await _wishListItem.GetAllWishListItemAsync(UserId);

        if(!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{wishListItemId}")]
    public async Task<IActionResult> GetWishListItemByIdAsync(int wishListItemId)
    {
        var result = await _wishListItem.GetWishListItemById(UserId, wishListItemId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{productId}")]

    public async Task<IActionResult> AddItemToWishListAsync(int productId)
    {
        var result = await _wishListItem.AddItemToWishListAsync(productId, UserId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return CreatedAtAction(nameof(GetWishListItemByIdAsync), new {wishListItemId = result.Value.Id}, result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItemFromWishListAsync(int id)
    {
        var result = await _wishListItem.DeleteItemFromWishListAsync(id, UserId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}