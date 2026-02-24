using MyFirstProject.Models;

namespace MyFirstProject.Repositories;

public interface IReviewRepository
{
    Task<List<Review>> GetReviewsAsync(ReviewQueryParameters parameters, int productId);
    Task<Review> GetReviewAsync(int reviewId, string userId);
    Task AddReviewAsync(Review reviews);
    bool DeleteReview(Review review);
}