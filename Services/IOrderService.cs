using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IOrderService
{
    Task<Result<OrderDto>> GetOrderAsync(string userId, int id);
    Task<Result<OrderDto>> CreateOrderFromCartAsync(string userId);
}