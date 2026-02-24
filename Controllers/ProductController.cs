using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MyFirstProject.Models;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
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
            return BadRequest(product.Error);
        }

        return Ok(product.Value);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductByIdAsync(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);

        if (!product.IsSuccess)
        {
            return NotFound(product.Error);
        }

        return Ok(product.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var newProduct = await _productService.CreateProductAsync(createProductDto);

        if (!newProduct.IsSuccess)
        {
            return BadRequest(newProduct.Error);
        }
        return CreatedAtAction(nameof(GetProductByIdAsync), new { productId = newProduct.Value.Id }, newProduct);
    }

    [HttpPatch("{productId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> PatchProduct(int productId, [FromBody] ProductDto newProductDto)
    {
        var patchedProduct = await _productService.PatchProductAsync(productId, newProductDto);

        if (!patchedProduct.IsSuccess)
        {
            return BadRequest(patchedProduct.Error);
        }

        string cacheKey = $"product_{productId}";
        await _cache.RemoveAsync(cacheKey);

        return NoContent();
    }

    [HttpDelete("{productId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var deleteProduct = await _productService.DeleteProductAsync(productId);

        if(!deleteProduct.IsSuccess)
        {
            return NotFound(deleteProduct.Error);
        }

        return NoContent();
    }
}