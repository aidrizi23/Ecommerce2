using AuthAlbiWebSchool.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthAlbiWebSchool.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public ReviewRepository(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<Review> GetReviewByIdAsync(int reviewId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == reviewId);
    }

    public async Task<Review> CreateReviewAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        
        await _context.Entry(review)
            .Reference(r => r.User)
            .LoadAsync();
        
        await _context.Entry(review)
            .Reference(r => r.Product)
            .LoadAsync();
        
        return review;
    }

    public async Task<Review> UpdateReviewAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
        
        await _context.Entry(review)
            .Reference(r => r.User)
            .LoadAsync();
        
        await _context.Entry(review)
            .Reference(r => r.Product)
            .LoadAsync();
        
        return review;
    }


    public async Task DeleteReviewAsync(Review review)
    {
        if (review != null)
        {
            review.Product.Reviews.Remove(review);
            review.User.Reviews.Remove(review);
            _context.Remove(review);
        }
            
        await _context.SaveChangesAsync();
    }
    
    public async Task SoftDeleteReviewAsync(Review review)
    {
        if(review != null)
            review.IsDeleted = true;
            await _context.SaveChangesAsync();
    }
    
    
}

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
    Task<Review> GetReviewByIdAsync(int reviewId);
    Task<Review> CreateReviewAsync(Review review);
    Task<Review> UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(Review review);
    Task SoftDeleteReviewAsync(Review review);
}

