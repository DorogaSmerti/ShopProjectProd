using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MyFirstProject.Models;

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
        var products = await _unitOfWork.Products.GetAllProduct(parameters);
        
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
        string cacheKey = $"product_{id}";

        var cachedProduct = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            return Result<ProductDto>.Success(JsonSerializer.Deserialize<ProductDto>(cachedProduct));
        }

        var product = await _unitOfWork.Products.GetByIdProduct(id);

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

        await _cache.SetStringAsync(cacheKey, serializedProduct, cacheOptions);
        
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

        await _unitOfWork.Products.AddProduct(newProduct);
        await _unitOfWork.CompleteAsync();

        var productDto = new ProductDto
        {
            Id = newProduct.Id,
            Name = newProduct.Name,
            Price = newProduct.Price,
            Description = newProduct.Description,
            Stock = newProduct.Stock,
            Reviews = null
        };

        return Result<ProductDto>.Success(productDto);
    }

    public async Task<Result<ProductDto>> PatchProductAsync(int id, ProductDto newProductDto)
    {
        var product = await _unitOfWork.Products.GetByIdProduct(id);

        if (product == null)
        {
            return Result<ProductDto>.Failure(DomainErrors.Product.ProductNotFound);
        }

        product.Name = newProductDto.Name;
        product.Price = newProductDto.Price;
        product.Stock = newProductDto.Stock;
        product.Description = newProductDto.Description;

        _unitOfWork.Products.UpdateProduct(product);
        
        await _unitOfWork.CompleteAsync();

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
        return Result<ProductDto>.Success(productDto);
    } 

    public async Task<Result<bool>> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdProduct(id);

        if (product == null)
        {
            return Result<bool>.Failure(DomainErrors.Product.ProductNotFound);
        }

        _unitOfWork.Products.DeleteProduct(product);
        await _unitOfWork.CompleteAsync();
        return Result<bool>.Success(true);
    }
}