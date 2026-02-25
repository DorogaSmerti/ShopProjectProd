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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _wishListItem.GetAllWishListItemAsync(userId);

        if(!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }
    [HttpPost("{productId}")]

    public async Task<IActionResult> AddItemToWishListAsync(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _wishListItem.AddItemToWishListAsync(productId, userId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItemFromWishListAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _wishListItem.DeleteItemFromWishListAsync(id, userId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}