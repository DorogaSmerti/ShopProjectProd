using MyFirstProject.Services;

public static class DomainErrors
{
    public static class WishList
    {
        public static readonly Error LimitReached = new("WishList.LimitReached", "Лимит желаемых товаров достигнут");
        public static readonly Error ItemNotFound = new("WishList.ItemNotFound", "Товар не найден в списке желаемых товаров");
    }

    public static class Product
    {
        public static readonly Error NotFound = new("Product.NotFound", "Товар не найден");
    }
}