using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MyFirstProject.Constants;
using MyFirstProject.Models;
using Serilog;

namespace MyFirstProject.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    public readonly IDistributedCache _cache;
    public ProductService(IUnitOfWork unitOfWork, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<List<ProductDto>>> GetAllProductAsync(ProductQueryParameters parameters)
    {
        var products = await _unitOfWork.Product.GetAllProduct(parameters);
        
        var result = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            Stock = p.Stock,
        }).ToList();

        return Result<List<ProductDto>>.Success(result);
    }

    public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
    {
        var cachedProduct = await _cache.GetStringAsync(CachedKeys.Product(id));

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            Log.Information("Product with id {ProductId} retrieved from cache.", id);
            return Result<ProductDto>.Success(JsonSerializer.Deserialize<ProductDto>(cachedProduct));
        }

        var product = await _unitOfWork.Product.GetByIdProduct(id);

        if (product == null)
        {
            return Result<ProductDto>.Failure(DomainErrors.Product.ProductNotFound);
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Stock = product.Stock,
            Reviews = product.Reviews.Select(r => new ReviewsDto
            {
                Id = r.Id,
                Body = r.Body,
                Rating = r.Rating,
                CreatedAt = r.CreateAt,
                Username = r.User?.UserName ?? "Аноним",
            }).ToList()
        };

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        };

        var serializedProduct = JsonSerializer.Serialize(productDto);

        await _cache.SetStringAsync(CachedKeys.Product(id), serializedProduct, cacheOptions);
        
        return Result<ProductDto>.Success(productDto);
    }

    public async Task<Result<ProductDto>> CreateProductAsync(CreateProductDto createProductDto)
    {
        var newProduct = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            Stock = createProductDto.Stock,
            CreateAt = DateTime.UtcNow
        };

        await _unitOfWork.Product.AddProduct(newProduct);
        await _unitOfWork.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = newProduct.Id,
            Name = newProduct.Name,
            Price = newProduct.Price,
            Description = newProduct.Description,
            Stock = newProduct.Stock,
        };

        return Result<ProductDto>.Success(productDto);
    }

    public async Task<Result<ProductDto>> PatchProductAsync(int id, ProductDto newProductDto)
    {
        var product = await _unitOfWork.Product.GetByIdProduct(id);

        if (product == null)
        {
            return Result<ProductDto>.Failure(DomainErrors.Product.ProductNotFound);
        }

        product.Name = newProductDto.Name;
        product.Price = newProductDto.Price;
        product.Stock = newProductDto.Stock;
        product.Description = newProductDto.Description;

        _unitOfWork.Product.UpdateProduct(product);
        
        await _unitOfWork.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Stock = product.Stock,
            Reviews = product.Reviews.Select(r => new ReviewsDto
            {
                Id = r.Id,
                Body = r.Body,
                Rating = r.Rating,
                CreatedAt = r.CreateAt,
                Username = r.User?.UserName ?? "Аноним",
            }).ToList()
        };

        await _cache.RemoveAsync(CachedKeys.Product(id));

        return Result<ProductDto>.Success(productDto);
    } 

    public async Task<Result<ProductDto>> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Product.GetByIdProduct(id);

        if (product == null)
        {
            return Result<ProductDto>.Failure(DomainErrors.Product.ProductNotFound);
        }

        _unitOfWork.Product.DeleteProduct(product);
        await _unitOfWork.SaveChangesAsync();

        await _cache.RemoveAsync(CachedKeys.Product(id));

        return Result<ProductDto>.Success();
    }
}