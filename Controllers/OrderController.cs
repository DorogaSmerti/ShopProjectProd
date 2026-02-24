using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var order = await _orderService.GetOrderAsync(userId, id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderFromCartAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }  

        var result = await _orderService.CreateOrderFromCartAsync(userId);

        if (result.Status == OrderResult.ItemNotFound)
        {
            return NotFound("Предмет не найден в корзине");
        }
        if (result.Status == OrderResult.ProductNotFound)
        {
            return BadRequest(new { message = "Товар не найден" });
        }

        if (result.Status == OrderResult.NotEnoughStock)
        {
            return BadRequest(new { message = "Недостаточно товара на складе" });
        }
        return CreatedAtAction(nameof(GetOrderAsync), new {id = result.orderDto.Id}, result.orderDto);
    }
} 

