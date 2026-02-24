using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Models;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class CartController : ControllerBase
{
    private readonly ICartService _cartServices;

    public CartController(ICartService cartServices)
    {
        _cartServices = cartServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetCartAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }  

        var cart = await _cartServices.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCartAsync([FromBody] AddToCartDto addToCartDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }          
        var cart = await _cartServices.AddToCartAsync(userId, addToCartDto.ProductId, addToCartDto.Quantity);

        if (cart.cartResult == CartResult.ProductNotFound)
        {
            return NotFound(new { message = "Товар не найден" });
        }
        if (cart.cartResult ==  CartResult.NotEnoughStock)
        {
            return BadRequest(new { message = "Недостаточно товара на складе" });
        }
        return Ok(new { message = "Товар добавлен в корзину", cart.cartItemDto});
    }
    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> DeleteFromCartAsync(int cartItemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }  
               
        var cart = await _cartServices.DeleteFromCartAsync(userId, cartItemId);

        if (cart == CartResult.ItemNotFound)
        {
            return NotFound(new { message = "Товар не найден в корзине" });
        }
        return NoContent();
    }
    [HttpPut("{cartItemId}")]
    public async Task<IActionResult> UpdateQuantityAsync(int cartItemId, [FromBody] ChangeQuantityDto changeQuantityDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }          
        var cart = await _cartServices.UpdateQuantityAsync(userId, cartItemId, changeQuantityDto.Quantity);

        if (cart == CartResult.ProductNotFound)
        {
            return NotFound(new { message = "Товар не найден" });
        }

        if (cart == CartResult.ItemNotFound)
        {
            return NotFound(new { message = "Товар не найден в корзине" });
        }

        if (cart == CartResult.NotEnoughStock)
        {
            return BadRequest(new { message = "Недостаточно товара на складе" });
        }

        return NoContent();
    }
}
