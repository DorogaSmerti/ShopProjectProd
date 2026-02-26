namespace MyFirstProject.Constants;

public static class CachedKeys
{
    public static string Cart(string userId) => $"Cart_{userId}";
    public static string Product(int productId) => $"Product_{productId}";
    public static string Review(int reviewId) => $"Review_{reviewId}";
}