using Microsoft.EntityFrameworkCore;
using MyFirstProject.Data;
using MyFirstProject.Models;

namespace MyFirstProject.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProduct(ProductQueryParameters parameters)
    {
        IQueryable<Product> products = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            products = products.Where(p => p.Name.ToLower().Contains(parameters.SearchTerm.ToLower()));
        }

        products  = products.OrderBy(p => p.Id);

        products = products.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                           .Take(parameters.PageSize);

        return await products.ToListAsync();
    }

    public async Task<Product> GetByIdProduct(int id)
    {
        return await _context.Products.Include(p => p.Reviews).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddProduct(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void UpdateProduct(Product product)
    {
        _context.Products.Update(product);
    }
    
    public void DeleteProduct(Product product)
    {
        _context.Products.Remove(product);
    }
}