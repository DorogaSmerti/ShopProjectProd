using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IReviewService
{
    Task<List<ReviewsDto>> GetReviewsForProductAsync(ReviewQueryParameters parameters, int productId);
    Task<ReviewsDto?> AddReviewAsync(int productId, string userId, CreateReviewDto createReviewDto);
    Task<bool> DeleteReviewAsync(int reviewId, string userId);
}