public enum CartResult
{
    Success,
    ProductNotFound,
    ItemNotFound,
    NotEnoughStock
}

public enum OrderResult
{
    Success,
    ProductNotFound,
    ItemNotFound,
    NotEnoughStock,
    BadRequest,
    OrderNotFound
}

public enum OrderStatus
{
    Pending,
    Processing,
    InTransit,
    Delivered,
    Canceled
    
}

public enum UserResult
{
    UserNotFound,
    UserHasThisRole,
    Success,
    Failure
}

public enum WishListResult
{
    S
}