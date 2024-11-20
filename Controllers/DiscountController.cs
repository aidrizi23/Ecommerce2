using System.Security.Claims;
using AuthAlbiWebSchool.Data;
using AuthAlbiWebSchool.Models;
using AuthAlbiWebSchool.Models.Discount;
using AuthAlbiWebSchool.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthAlbiWebSchool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        
        private readonly IDiscountRepository _discountRepo;
        private readonly IProductRepository _productRepository;
        
        public DiscountController(IDiscountRepository discountRepo, IProductRepository productRepository)
        {
            _discountRepo = discountRepo;
            _productRepository = productRepository;
        }
        
        // Get all active discounts
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<Discount>>>> GetAllActiveDiscounts()
        {
            try
            {
                var currentDate = DateTime.Now;
                var discounts = await _discountRepo.GetActiveDiscountsAsync(currentDate);

                return Ok(new ApiResponse<List<Discount>>
                {
                    Success = true,
                    Data = discounts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Discount>>
                {
                    Success = false,
                    Message = "An error occurred while fetching discounts."
                });
            }
            
        }
        
        
        // Get active discounts for a product
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<Discount>>>> GetActiveDiscountsForProduct(int productId)
        {
            try
            {
                var currentDate = DateTime.Now;
                var discounts = await _discountRepo.GetActiveDiscountsForProductAsync(productId, currentDate);

                return Ok(new ApiResponse<List<Discount>>
                {
                    Success = true,
                    Data = discounts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Discount>>
                {
                    Success = false,
                    Message = "An error occurred while fetching discounts for the product."
                });
            }
        }

        // Get active discounts for a user
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<Discount>>>> GetActiveDiscountsForUser(string userId)
        {
            try
            {
                var currentDate = DateTime.Now;
                var discounts = await _discountRepo.GetActiveDiscountsForUserAsync(userId, currentDate);

                return Ok(new ApiResponse<List<Discount>>
                {
                    Success = true,
                    Data = discounts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Discount>>
                {
                    Success = false,
                    Message = "An error occurred while fetching discounts for the user."
                });
            }
        }
        
        // Apply a discount to a product
        [HttpPost("apply-to-product")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> ApplyDiscountToProduct([FromBody] ApplyDiscountToProductDto dto)
        {
            try
            {
                // Ensure the current user is the seller
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var product = await _productRepository.GetProductByIdAsync(dto.ProductId);

                if (product == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Product not found.",
                        Data = null
                    });
                }

                if (product.SellerId != userId)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "You are not authorized to apply a discount to this product.",
                        Data = null
                    });
                }

                // Apply the discount
                await _discountRepo.ApplyDiscountToProductAsync(dto.ProductId, dto.DiscountId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Discount applied to the product successfully.",
                    Data = null // No need to return any data on success
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while applying the discount.",
                    Data = null // No data to return on failure
                });
            }
        }

        
        // Apply a discount to a user
        [HttpPost("apply-to-user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> ApplyDiscountToUser([FromBody] ApplyDiscountToUserDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Ensure the current user is the one applying the discount to their own account
                if (userId != dto.UserId)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "You can only apply discounts to your own account.",
                        Data = null
                    });
                }

                // Apply the discount to the user
                await _discountRepo.ApplyDiscountToUserAsync(dto.UserId, dto.DiscountId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Discount applied to the user successfully.",
                    Data = null // No data to return on success
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while applying the discount.",
                    Data = null // No data to return on failure
                });
            }
        }

        
        
        
    }
}
