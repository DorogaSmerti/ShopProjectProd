using MyFirstProject.Models;

namespace MyFirstProject.Repositories;
public interface IOrderRepository
{
    Task<Order?> GetOrderAsync(string userId, int orderId);
    Task CreateOrderFromCartAsync(Order order);
    Task AddToOrderItemAsync(OrderItem orderItem);
}