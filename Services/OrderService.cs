using MyFirstProject.Models;

namespace MyFirstProject.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderDto?> GetOrderAsync(string userId, int id)
    {
        var order = await _unitOfWork.Orders.GetOrderAsync(userId, id);

        if(order == null)
        {
            return null;
        }

        return new OrderDto
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            OrderItems = order.OrderItems.Select(p => new OrderItemDto
            {
                ProductId = p.ProductId,
                Price = p.Price,
                Quantity = p.Quantity,
            }).ToList()
        };
    }

    public async Task<(OrderResult Status, OrderDto? orderDto)> CreateOrderFromCartAsync(string userId)
    {
        var cartItems = await _unitOfWork.CartItem.GetCartAsync(userId);

        if (cartItems == null || cartItems.Count == 0)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (OrderResult.ItemNotFound, null);
            }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            decimal totalPrice = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in cartItems)
            {
                if (item.Product == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return (OrderResult.ProductNotFound, null);
                }

                if (item.Product.Stock < item.QuantityCartItem)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return (OrderResult.NotEnoughStock, null);
                }
                item.Product.Stock -= item.QuantityCartItem;
                totalPrice += (item.QuantityCartItem * item.Product.Price);

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.QuantityCartItem,
                    Price = item.Product.Price
                };
                orderItems.Add(orderItem);
            }

            var newOrder = new Order
            {
                Status = OrderStatus.Pending,
                TotalAmount = totalPrice,
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems
            };
            await _unitOfWork.Orders.CreateOrderFromCartAsync(newOrder);
            
            _unitOfWork.CartItem.RemoveRangeFromCart(cartItems);

            await _unitOfWork.CompleteAsync();

            await _unitOfWork.CommitTransactionAsync();

            var orderDto = new OrderDto
            {
                Id = newOrder.Id,
                OrderDate = newOrder.OrderDate,
                Status = newOrder.Status,
                TotalAmount = totalPrice,
                UserId = newOrder.UserId,
                OrderItems = cartItems.Select(item => new OrderItemDto
                {
                    Id = item.CartItemId,
                    Quantity = item.QuantityCartItem,
                    Price = item.Product.Price,
                    ProductId = item.ProductId
                }).ToList()
            };

            return (OrderResult.Success, orderDto);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании заказа");
            await _unitOfWork.RollbackTransactionAsync();
            return (OrderResult.BadRequest, null);
        }
    }
}