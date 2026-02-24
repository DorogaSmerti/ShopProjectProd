using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;
using System.Security.Claims;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class WishListController : ControllerBase
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
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var allWishItem = await _wishListItem.GetAllWishListItemAsync(userId);

        if(allWishItem == null)
        {
            return NotFound();
        }

        return Ok(allWishItem);
    }
    [HttpPost("{productId}")]

    public async Task<IActionResult> AddItemToWishListAsync(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _wishListItem.AddItemToWishListAsync(productId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItemFromWishListAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _wishListItem.DeleteItemFromWishListAsync(id, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Code = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}