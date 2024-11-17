using AuthAlbiWebSchool.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthAlbiWebSchool.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly ApplicationDbContext _context;
    
    public DiscountRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Discount> GetDiscountByIdAsync(int id)
    {
        return await _context.Discounts.FindAsync(id);
    }
    // Get all active discounts
    public async Task<List<Discount>> GetActiveDiscountsAsync(DateTime currentDate)
    {
        return await _context.Discounts
            .Where(d => d.IsActive && d.StartDate <= currentDate && d.EndDate >= currentDate)
            .ToListAsync();
    }
    
    // Add a new discount
    public async Task AddDiscountAsync(Discount discount)
    {
        await _context.Discounts.AddAsync(discount);
        await _context.SaveChangesAsync();
    }

    // Update an existing discount
    public async Task UpdateDiscountAsync(Discount discount)
    {
        _context.Discounts.Update(discount);
        await _context.SaveChangesAsync();
    }
    // Delete a discount
    public async Task DeleteDiscountAsync(int discountId)
    {
        var discount = await _context.Discounts.FindAsync(discountId);
        if (discount != null)
        {
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }
    }
    
    // Apply a discount to a product
    public async Task ApplyDiscountToProductAsync(int productId, int discountId)
    {
        var productDiscount = new ProductDiscount
        {
            ProductId = productId,
            DiscountId = discountId
        };

        await _context.ProductDiscounts.AddAsync(productDiscount);
        await _context.SaveChangesAsync();
    }
    
    // Apply a discount to a user
    public async Task ApplyDiscountToUserAsync(string userId, int discountId)
    {
        var userDiscount = new UserDiscount
        {
            UserId = userId,
            DiscountId = discountId
        };

        await _context.UserDiscounts.AddAsync(userDiscount);
        await _context.SaveChangesAsync();
    }
    
    // Remove a discount from a product
    public async Task RemoveDiscountFromProductAsync(int productId, int discountId)
    {
        var productDiscount = await _context.ProductDiscounts
            .FirstOrDefaultAsync(pd => pd.ProductId == productId && pd.DiscountId == discountId);

        if (productDiscount != null)
        {
            _context.ProductDiscounts.Remove(productDiscount);
            await _context.SaveChangesAsync();
        }
    }

    // Remove a discount from a user
    public async Task RemoveDiscountFromUserAsync(string userId, int discountId)
    {
        var userDiscount = await _context.UserDiscounts
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DiscountId == discountId);

        if (userDiscount != null)
        {
            _context.UserDiscounts.Remove(userDiscount);
            await _context.SaveChangesAsync();
        }
    }
    
    // Get active discounts for a specific product
    public async Task<List<Discount>> GetActiveDiscountsForProductAsync(int productId, DateTime currentDate)
    {
        var productDiscounts = await _context.ProductDiscounts
            .Where(pd => pd.ProductId == productId)
            .Include(pd => pd.Discount)
            .Where(pd => pd.Discount.IsActive && pd.Discount.StartDate <= currentDate && pd.Discount.EndDate >= currentDate)
            .Select(pd => pd.Discount)
            .ToListAsync();

        return productDiscounts;
    }
    
    // Get active discounts for a specific user
    public async Task<List<Discount>> GetActiveDiscountsForUserAsync(string userId, DateTime currentDate)
    {
        var userDiscounts = await _context.UserDiscounts
            .Where(ud => ud.UserId == userId)
            .Include(ud => ud.Discount)
            .Where(ud => ud.Discount.IsActive && ud.Discount.StartDate <= currentDate && ud.Discount.EndDate >= currentDate)
            .Select(ud => ud.Discount)
            .ToListAsync();

        return userDiscounts;
    }
    
    // Get all discounts applied to a product
    public async Task<List<ProductDiscount>> GetProductDiscountsAsync(int productId)
    {
        return await _context.ProductDiscounts
            .Where(pd => pd.ProductId == productId)
            .Include(pd => pd.Discount)
            .ToListAsync();
    }

    // Get all discounts applied to a user
    public async Task<List<UserDiscount>> GetUserDiscountsAsync(string userId)
    {
        return await _context.UserDiscounts
            .Where(ud => ud.UserId == userId)
            .Include(ud => ud.Discount)
            .ToListAsync();
    }
    
    
    
}

public interface IDiscountRepository
{
    Task<Discount> GetDiscountByIdAsync(int id);
    Task<List<Discount>> GetActiveDiscountsAsync(DateTime currentDate);
    Task AddDiscountAsync(Discount discount);
    Task UpdateDiscountAsync(Discount discount);
    Task DeleteDiscountAsync(int discountId);
    Task ApplyDiscountToProductAsync(int productId, int discountId);
    Task ApplyDiscountToUserAsync(string userId, int discountId);
    Task RemoveDiscountFromProductAsync(int productId, int discountId);
    Task RemoveDiscountFromUserAsync(string userId, int discountId);
    Task<List<Discount>> GetActiveDiscountsForProductAsync(int productId, DateTime currentDate);
    Task<List<Discount>> GetActiveDiscountsForUserAsync(string userId, DateTime currentDate);
    Task<List<ProductDiscount>> GetProductDiscountsAsync(int productId);
    Task<List<UserDiscount>> GetUserDiscountsAsync(string userId);
}