using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IOrderService
{
    Task<OrderDto?> GetOrderAsync(string userId, int id);
    Task<(OrderResult Status, OrderDto? orderDto)> CreateOrderFromCartAsync(string userId);
}