using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Models;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class CartController : ApiControllerBase
{
    private readonly ICartService _cartServices;

    public CartController(ICartService cartServices)
    {
        _cartServices = cartServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetCartAsync()
    {
        var cart = await _cartServices.GetCartAsync(UserId);

        if (!cart.IsSuccess)
        {
            return HandleFailure(cart.Error);
        }
        return Ok(cart.Value);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCartAsync([FromBody] AddToCartDto addToCartDto)
    {
        var cart = await _cartServices.AddToCartAsync(UserId, addToCartDto.ProductId, addToCartDto.Quantity);

        if (!cart.IsSuccess)
        {
            return HandleFailure(cart.Error);
        }

        return CreatedAtAction(nameof(GetCartAsync), new { userId = UserId }, cart.Value);
    }
    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> DeleteFromCartAsync(int cartItemId)
    {   
        var cart = await _cartServices.DeleteFromCartAsync(UserId, cartItemId);

        if (!cart.IsSuccess)
        {
            return HandleFailure(cart.Error);
        }

        return NoContent();
    }
    [HttpPut("{cartItemId}")]
    public async Task<IActionResult> UpdateQuantityAsync(int cartItemId, [FromBody] ChangeQuantityDto changeQuantityDto)
    {
        var cart = await _cartServices.UpdateQuantityAsync(UserId, cartItemId, changeQuantityDto.Quantity);

        if (!cart.IsSuccess)
        {
            return HandleFailure(cart.Error);
        }

        return NoContent();
    }
}
