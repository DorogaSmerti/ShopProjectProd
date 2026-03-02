using MyFirstProject.Models;

namespace MyFirstProject.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ReviewsDto>>> GetReviewsForProductAsync(ReviewQueryParameters parameters, int productId)
    {
        var reviews = await _unitOfWork.Reviews.GetReviewsAsync(parameters, productId);

        if (reviews == null)
        {
            return Result<List<ReviewsDto>>.Failure(DomainErrors.Review.ReviewNotFound);
        }

        var reviewsDto = reviews.Select(r => new ReviewsDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Username = r.User?.UserName ?? "Аноним",
            Body = r.Body,
            CreatedAt = r.CreateAt
        }).ToList();

        return Result<List<ReviewsDto>>.Success(reviewsDto);
    }
    public async Task<Result<ReviewsDto>> AddReviewAsync(int productId, string userId, CreateReviewDto createReviewDto)
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
        await _unitOfWork.SaveChangesAsync();

        var user = await _unitOfWork.Users.GetUserByIdAsync(userId);

        var result = new ReviewsDto
        {
            Id = review.Id,
            Username = user?.UserName ?? "Аноним",
            Body = review.Body,
            Rating = review.Rating,
        };

        return Result<ReviewsDto>.Success(result);
    }

    public async Task<Result<bool>> DeleteReviewAsync(int reviewId, string userId)
    {
        var review = await _unitOfWork.Reviews.GetReviewAsync(reviewId, userId);

        if (review == null)
        {
            return Result<bool>.Failure(DomainErrors.Review.ReviewNotFound);
        }

        _unitOfWork.Reviews.DeleteReview(review);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}