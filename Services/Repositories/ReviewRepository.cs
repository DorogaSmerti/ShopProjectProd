using Microsoft.EntityFrameworkCore;
using MyFirstProject.Data;
using MyFirstProject.Models;

namespace MyFirstProject.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetReviewsAsync(ReviewQueryParameters parameters, int productId)
    {
        IQueryable<Review> reviews = _context.Reviews.Where(p => p.ProductId == productId);

        if (parameters.MaxRating.HasValue)
        {
            reviews = reviews.Where(p => p.Rating <= parameters.MaxRating.Value);
        }
        
        reviews = reviews.OrderBy(p => p.Id);

        reviews = reviews.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                        .Take(parameters.PageSize);
                        

        return await reviews.ToListAsync();
    }

    public async Task<Review> GetReviewAsync(int reviewId, string userId)
    {
        return await _context.Reviews.FirstOrDefaultAsync(p => p.Id == reviewId && p.UserId == userId);
    }

    public async Task AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public bool DeleteReview(Review review)
    {
        _context.Reviews.Remove(review);
        return true;
    }
}