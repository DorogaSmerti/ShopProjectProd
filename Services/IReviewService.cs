using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IReviewService
{
    Task<Result<List<ReviewsDto>>> GetReviewsForProductAsync(ReviewQueryParameters parameters, int productId);
    Task<Result<ReviewsDto>> GetReviewByIdAsync(string userId, int reviewId);
    Task<Result<ReviewsDto>> AddReviewAsync(int productId, string userId, CreateReviewDto createReviewDto);
    Task<Result<bool>> DeleteReviewAsync(int reviewId, string userId);
}