using System.Security.Claims;
using AuthAlbiWebSchool.Data;
using AuthAlbiWebSchool.Models;
using AuthAlbiWebSchool.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthAlbiWebSchool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        
        
        private readonly IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;
        
        
        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        
        [HttpPost("add")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> AddToCart(AddToCartDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _productRepository.AddToCartAsync(userId, dto.ProductId, dto.Quantity);

                var cart = await _productRepository.GetCartAsync(userId);
                
                return Ok(new ApiResponse<CartResponseDto>
                {
                    Success = true,
                    Message = "Item added to cart successfully",
                    Data = cart
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<CartResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("remove-from-cart{productId}")]
        
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> RemoveFromCart(int productId)
        {
            
            // get the user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            
            // remove the product from the users
            try
            {
                await _productRepository.RemoveFromCartAsync(userId, productId);

                return Ok();
            }
            catch
            {
                return BadRequest("Could not remove the product from the cart.");
            }
            
        }
        
       
        
        [HttpPost("checkout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CheckoutCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _productRepository.CheckoutCartAsync(userId);

                if (result.Success)
                {
                    return Ok(new ApiResponse<int>
                    {
                        Success = true,
                        Message = "Order placed successfully",
                        Data = result.OrderId
                    });
                }

                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<int>
                {
                    Success = false,
                    Message = "An error occurred while processing the checkout."
                });
            }
        }
        
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> GetCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = await _productRepository.GetCartAsync(userId);

                return Ok(new ApiResponse<CartResponseDto>
                {
                    Success = true,
                    Data = cart
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CartResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while fetching the cart."
                });
            }
        }
    }
}
