using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Models;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/product/{productId}/reviews")]
[Authorize]
public class ReviewController : ApiControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviewsForProductAsync([FromQuery]ReviewQueryParameters parameters ,int productId)
    {
        var result = await _reviewService.GetReviewsForProductAsync(parameters, productId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{reviewId}")]
    public async Task<IActionResult> GetReviewByIdAsync(int reviewId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _reviewService.GetReviewByIdAsync(userId ?? string.Empty, reviewId);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> AddReviewAsync(int productId, CreateReviewDto createReviewDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _reviewService.AddReviewAsync(productId, userId, createReviewDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }
        return CreatedAtAction(nameof(GetReviewByIdAsync), new {reviewId = result.Value.Id}, result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var review = await _reviewService.DeleteReviewAsync(reviewId, userId);

        if (!review.IsSuccess)
        {
            return HandleFailure(review.Error);
        }
        return NoContent();
    }
}