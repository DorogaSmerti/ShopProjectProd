using MyFirstProject.Models;
using Serilog;

namespace MyFirstProject.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        await _unitOfWork.BeginTransactionAsync();
        try
        {
        var cartItems = await _unitOfWork.CartItem.GetCartAsync(userId);

        if (cartItems == null || cartItems.Count == 0)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<OrderDto>.Failure(DomainErrors.Order.CartEmpty);
            }
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

            await _unitOfWork.CommitAsync();

            var orderDto = new OrderDto
            {
                Id = newOrder.Id,
                OrderDate = newOrder.OrderDate,
                Status = newOrder.Status,
                TotalAmount = totalPrice,
                UserId = newOrder.UserId,
                OrderItems = newOrder.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ProductId = item.ProductId
                }).ToList()
            };

            return Result<OrderDto>.Success(orderDto);
        }

        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при создании заказа");
            await _unitOfWork.RollbackTransactionAsync();
            return Result<OrderDto>.Failure(DomainErrors.Order.OrderCreationFailed);
        }
    }
}