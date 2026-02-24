using MyFirstProject.Models;

namespace MyFirstProject.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllProduct(ProductQueryParameters parameters);
    Task<Product> GetByIdProduct(int id);
    Task AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
}