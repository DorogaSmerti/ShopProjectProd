using MyFirstProject.Services;
using MyFirstProject.Models;

public static class DomainErrors
{
    public static class WishList
    {
        public static readonly Error LimitReached = new("WishList.LimitReached", "Лимит желаемых товаров достигнут");
        public static readonly Error ItemNotFound = new("WishList.ItemNotFound", "Товар не найден в списке желаемых товаров");
    }

    public static class Product
    {
        public static readonly Error ProductNotFound = new("Product.NotFound", "Product not found");
    }

    public static class Cart
    {
        public static readonly Error CartNotFound = new("Cart.NotFound", "Cart not found");
        public static readonly Error ProductNotFound = new("Cart.ProductNotFound", "Product not found");
        public static readonly Error NotEnoughStock = new("Cart.NotEnoughStock", "Not enough stock");
        public static readonly Error ItemNotFound = new("Cart.ItemNotFound", "Item not found in cart");
    }

    public static class Order
    {
        public static readonly Error OrderNotFound = new("Order.NotFound", "Order not found");
        public static readonly Error CartEmpty = new("Order.CartEmpty", "Cart is empty");
        public static readonly Error InsufficientStock = new("Order.InsufficientStock", "Not enough stock for one or more items in the cart");
        public static readonly Error ProductNotFound = new("Order.ProductNotFound", "Product not found");
        public static readonly Error NotEnoughStock = new("Order.NotEnoughStock", "Not enough stock for one or more items in the cart");
        public static readonly Error OrderCreationFailed = new("Order.OrderCreationFailed", "Failed to create order");
    }

    public static class Review
    {
        public static readonly Error ProductNotFound = new("Review.ProductNotFound", "Product not found");
        public static readonly Error ReviewNotFound = new("Review.ReviewNotFound", "Review not found");
    }
}