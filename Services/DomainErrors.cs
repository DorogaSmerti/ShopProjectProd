using MyFirstProject.Services;

public static class DomainErrors
{
    public static class WishList
    {
        public static readonly Error LimitReached = new("WishList.LimitReached", "limit of 500 items in wishList reached");
        public static readonly Error ItemNotFound = new("WishList.ItemNotFound", "product not found in wishList");
        public static readonly Error WishListNotFound = new("WishList.WishListNotFound", "wishList not found");
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
    }

    public static class Order
    {
        public static readonly Error OrderNotFound = new("Order.NotFound", "Order not found");
        public static readonly Error CartEmpty = new("Order.CartEmpty", "Cart is empty");
        public static readonly Error ProductNotFound = new("Order.ProductNotFound", "Product not found");
        public static readonly Error NotEnoughStock = new("Order.NotEnoughStock", "Not enough stock for one or more items in the cart");
        public static readonly Error OrderCreationFailed = new("Order.OrderCreationFailed", "Failed to create order");
    }

    public static class Review
    {
        public static readonly Error ReviewNotFound = new("Review.ReviewNotFound", "Review not found");
    }

    public static class User
    {
        public static readonly Error UserNotFound = new("User.UserNotFound", "User not found");
        public static readonly Error UserHasThisRole = new("User.UserHasThisRole", "User already has this role");
        public static readonly Error UserRoleChangeFailed = new("User.UserRoleChangeFailed", "Failed to change user role");
        public static readonly Error UserAlreadyExists = new("User.UserAlreadyExists", "User with this username already exists");
        public static readonly Error UserCannotRegister = new("User.UserCannotRegister", "User cannot register with provided data");
        public static readonly Error EmailAlreadyExists = new("User.EmailAlreadyExists", "User with this email already exists");
        public static readonly Error PasswordOrUsernameDoesNotMatch = new("User.PasswordOrUsernameDoesNotMatch", "Password or username does not match");
        public static readonly Error UserDoesNotHaveThisRole = new("User.UserDoesNotHaveThisRole", "User doesn't have this role");
    }
}