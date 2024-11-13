using AuthAlbiWebSchool.Data;
using AuthAlbiWebSchool.Models;
using AuthAlbiWebSchool.Models.Reviews;
using AuthAlbiWebSchool.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace AuthAlbiWebSchool.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly UserManager<User> _userManager;
    
    public ReviewController(IReviewRepository reviewRepository,IProductRepository productRepository, UserManager<User> userManager)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> Index(int productId)
    {
        // get the reviews of the product
        var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
        
        // map the apiResponse to the view model
        var reviewDtos = reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            ReviewerName = r.User.UserName,
            ProductName = r.Product?.Name,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
            
        }).ToList();
        
        return Ok(new ApiResponse<List<ReviewDto>>()
        {
            Message = "Reviews fetched successfully",
            Success = true,
            Data = reviewDtos
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(ReviewForCreateDto dto)
    {
        // get the current user
        var user = await _userManager.GetUserAsync(User);
        
        // get the product
        var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
        
        // create the review
        var review = new Review
        {
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = dto.CreatedAt,
            ProductId = dto.ProductId,
            UserId = user.Id,
            UpdatedAt = null
        };
        
        // save the review
        var createdReview = await _reviewRepository.CreateReviewAsync(review);
        
        // map the apiResponse to the view model
        var reviewDto = new ReviewDto
        {
            Id = createdReview.Id,
            Rating = createdReview.Rating,
            Comment = createdReview.Comment,
            ReviewerName = user.UserName,
            ProductName = product.Name,
            CreatedAt = createdReview.CreatedAt,
            UpdatedAt = createdReview.UpdatedAt
        };
        
        return Ok(new ApiResponse<ReviewDto>()
        {
            Message = "Review created successfully",
            Success = true,
            Data = reviewDto
        });
        
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update(int id, ReviewForCreateDto dto)
    {
        // get the review
        var review = await _reviewRepository.GetReviewByIdAsync(id);
        
        // get the current user
        var user = await _userManager.GetUserAsync(User);
        
        // check if the review exists
        if (review == null)
        {
            return NotFound(new ApiResponse<ReviewDto>()
            {
                Message = "Review not found",
                Success = false,
                Data = null
            });
        }
        
        // check if the user is the owner of the review
        if (review.UserId != user.Id)
        {
            return Unauthorized(new ApiResponse<ReviewDto>()
            {
                Message = "You are not authorized to update this review",
                Success = false,
                Data = null
            });
        }
        
        // update the review
        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.UpdatedAt = DateTimeOffset.Now;
        
        // save the review
        var updatedReview = await _reviewRepository.UpdateReviewAsync(review);
        
        // map the apiResponse to the view model
        var reviewDto = new ReviewDto
        {
            Id = updatedReview.Id,
            Rating = updatedReview.Rating,
            Comment = updatedReview.Comment,
            ReviewerName = user.UserName,
            ProductName = updatedReview.Product.Name,
            CreatedAt = updatedReview.CreatedAt,
            UpdatedAt = updatedReview.UpdatedAt
        };
        
        return Ok(new ApiResponse<ReviewDto>()
        {
            Message = "Review updated successfully",
            Success = true,
            Data = reviewDto
        });
    }
    
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        // get the review
        var review = await _reviewRepository.GetReviewByIdAsync(id);
        
        // get the current user
        var user = await _userManager.GetUserAsync(User);
        
        // check if the review exists
        if (review == null)
        {
            return NotFound(new ApiResponse<ReviewDto>()
            {
                Message = "Review not found",
                Success = false,
                Data = null
            });
        }
        
        // check if the user is the owner of the review
        if (review.UserId != user.Id)
        {
            return Unauthorized(new ApiResponse<ReviewDto>()
            {
                Message = "You are not authorized to delete this review",
                Success = false,
                Data = null
            });
        }
        
        // delete the review
        await _reviewRepository.DeleteReviewAsync(review);
        
        return Ok(new ApiResponse<ReviewDto>()
        {
            Message = "Review deleted successfully",
            Success = true,
            Data = null
        });
    }
    
}