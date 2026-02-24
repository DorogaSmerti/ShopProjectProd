using MyFirstProject.Models;

namespace MyFirstProject.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReviewsDto>> GetReviewsForProductAsync(ReviewQueryParameters parameters, int productId)
    {
        var reviews = await _unitOfWork.Reviews.GetReviewsAsync(parameters, productId);

        return reviews.Select(r => new ReviewsDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Username = r.User?.UserName ?? "Аноним",
            Body = r.Body,
            CreateAt = r.CreateAt
        }).ToList();
    }
    public async Task<ReviewsDto?> AddReviewAsync(int productId, string userId, CreateReviewDto createReviewDto)
    {
        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = createReviewDto.Rating,
            Body = createReviewDto.Body,
            CreateAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddReviewAsync(review);
        await _unitOfWork.CompleteAsync();

        var user = await _unitOfWork.Users.GetUserByIdAsync(userId);

        return new ReviewsDto
        {
            Id = review.Id,
            Username = user?.UserName ?? "Аноним",
            Body = review.Body,
            Rating = review.Rating,
        };
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
    {
        var review = await _unitOfWork.Reviews.GetReviewAsync(reviewId, userId);

        if (review == null)
        {
            return false;
        }

        _unitOfWork.Reviews.DeleteReview(review);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}