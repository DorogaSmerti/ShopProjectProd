using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IProductService
{
    Task<Result<List<ProductDto>>> GetAllProductAsync(ProductQueryParameters parameters);
    Task<Result<ProductDto>> GetProductByIdAsync(int id);
    Task<Result<ProductDto>> CreateProductAsync(CreateProductDto newProductDto);
    Task<Result<ProductDto>> PatchProductAsync(int id, ProductDto newProductDto);
    Task<Result<bool>> DeleteProductAsync(int id);
}