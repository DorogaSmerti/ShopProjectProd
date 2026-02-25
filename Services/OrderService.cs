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

    public async Task<Result<OrderDto>> GetOrderAsync(string userId, int id)
    {
        var result = await _unitOfWork.Orders.GetOrderAsync(userId, id);

        if(result == null)
        {
            return Result<OrderDto>.Failure(DomainErrors.Order.OrderNotFound);
        }

        var orderDto = new OrderDto
        {
            Id = result.Id,
            TotalAmount = result.TotalAmount,
            Status = result.Status,
            UserId = result.UserId,
            OrderDate = result.OrderDate,
            OrderItems = result.OrderItems.Select(p => new OrderItemDto
            {
                ProductId = p.ProductId,
                Price = p.Price,
                Quantity = p.Quantity,
            }).ToList()
        };

        return Result<OrderDto>.Success(orderDto);
    }

    public async Task<Result<OrderDto>> CreateOrderFromCartAsync(string userId)
    {
        var cartItems = await _unitOfWork.CartItem.GetCartAsync(userId);

        if (cartItems == null || cartItems.Count == 0)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<OrderDto>.Failure(DomainErrors.Order.CartEmpty);
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
                    return Result<OrderDto>.Failure(DomainErrors.Order.ProductNotFound);
                }

                if (item.Product.Stock < item.QuantityCartItem)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<OrderDto>.Failure(DomainErrors.Order.NotEnoughStock);
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

            return Result<OrderDto>.Success(orderDto);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании заказа");
            await _unitOfWork.RollbackTransactionAsync();
            return Result<OrderDto>.Failure(DomainErrors.Order.OrderCreationFailed);
        }
    }
}