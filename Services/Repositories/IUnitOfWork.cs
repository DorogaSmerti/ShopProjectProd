using MyFirstProject.Repositories;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICartRepository CartItem { get; }
    IOrderRepository Orders { get; }
    IReviewRepository Reviews { get; }
    IUsersRepository Users { get; }
    IWishListItemRepository WishListItem { get; }

    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

