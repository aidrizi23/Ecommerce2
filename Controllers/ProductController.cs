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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;

        public ProductController(IProductRepository productRepository, UserManager<User> userManager)
        {
            _productRepository = productRepository;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    Condition = p.Condition,
                    Brand = p.Brand,
                    SellerName = p.Seller?.FullName,
                    CategoryName = p.Category?.Name
                }).ToList();

                return Ok(new ApiResponse<List<ProductDto>>
                {
                    Success = true,
                    Data = productDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ProductDto>>
                {
                    Success = false,
                    Message = "An error occurred while fetching products."
                });
            }
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Condition = product.Condition,
                Brand = product.Brand,
                SellerName = product.Seller?.FullName,
                CategoryName = product.Category?.Name
            };

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Data = productDto
            });
        }
    }

}
