using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class OrderController : ApiControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        var order = await _orderService.GetOrderAsync(UserId, id);

        if (!order.IsSuccess)
        {
            return HandleFailure(order.Error);
        }

        return Ok(order.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderFromCartAsync()
    {
        var result = await _orderService.CreateOrderFromCartAsync(UserId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }
        
        return CreatedAtAction(nameof(GetOrderAsync), new {id = result.Value.Id}, result.Value);
    }
} 

