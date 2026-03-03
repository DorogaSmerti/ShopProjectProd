using Microsoft.EntityFrameworkCore.Storage;
using MyFirstProject.Data;
using MyFirstProject.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _currentTransaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Product = new ProductRepository(_context);
        CartItem = new CartRepository(_context);
        Orders = new OrderRepository(_context);
        Reviews = new ReviewRepository(_context);
        Users = new UsersRepository(_context);
        WishListItem = new WishListItemRepository(_context);
        
    }

    public IProductRepository Product {get;}
    public ICartRepository CartItem { get; }
    public IOrderRepository Orders { get; } 
    public IReviewRepository Reviews { get; }
    public IUsersRepository Users { get; }
    public IWishListItemRepository WishListItem { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            return;
        }
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync(); // Сохраняем изменения перед коммитом
            
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}