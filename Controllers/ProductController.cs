using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MyFirstProject.Models;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ApiControllerBase
{
    private readonly IProductService _productService;
    private readonly IDistributedCache _cache;
    public ProductsController(IProductService productService, IDistributedCache cache)
    {
        _productService = productService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProduct([FromQuery] ProductQueryParameters parameters)
    {
        var product = await _productService.GetAllProductAsync(parameters);

        if(!product.IsSuccess)
        {
            return HandleFailure(product.Error);
        }

        return Ok(product.Value);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductByIdAsync(int productId)
    {
        var result = await _productService.GetProductByIdAsync(productId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto createProductDto)
    {
        var result = await _productService.CreateProductAsync(createProductDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }
        return CreatedAtAction(nameof(GetProductByIdAsync), new { productId = result.Value.Id }, result.Value);
    }

    [HttpPatch("{productId}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> PatchProductAsync(int productId, [FromBody] ProductDto newProductDto)
    {
        var result = await _productService.PatchProductAsync(productId, newProductDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        string cacheKey = $"product_{productId}";
        await _cache.RemoveAsync(cacheKey);

        return NoContent();
    }

    [HttpDelete("{productId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteProductAsync(int productId)
    {
        var result = await _productService.DeleteProductAsync(productId);

        if(!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}