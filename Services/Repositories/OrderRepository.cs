using Microsoft.EntityFrameworkCore;
using MyFirstProject.Data;
using MyFirstProject.Models;

namespace MyFirstProject.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderAsync(string userId, int orderId)
    {
        return await _context.Orders.AsNoTracking().Include(p => p.OrderItems).FirstOrDefaultAsync(p => p.UserId == userId && p.Id == orderId);
    }

    public async Task CreateOrderFromCartAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task AddToOrderItemAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
    }
}