using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Models;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/product/{productId}/reviews")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetReviewsForProductAsync([FromQuery]ReviewQueryParameters parameters ,int productId)
    {
        var reviews = await _reviewService.GetReviewsForProductAsync(parameters, productId);

        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> AddReviewAsync(int productId, CreateReviewDto createReviewDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var review = await _reviewService.AddReviewAsync(productId, userId, createReviewDto);
        return Ok(review);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var review = await _reviewService.DeleteReviewAsync(reviewId, userId);

        if (review == false)
        {
            return NotFound();
        }
        return NoContent();
    }
}