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
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts(int page = 1, int pageSize = 10)
        {
            try
            {
                var products = await _productRepository.GetAllPaginatedProductsAsync(page, pageSize);
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
        
        
        [HttpPost("buy-now")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> BuyNow(BuyNowDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _productRepository.BuyNowAsync(userId, dto.ProductId, dto.Quantity);

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
                    Message = "An error occurred while processing the order."
                });
            }
        }
        
        
        // CRUD operations for products will be added here only for sellers

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct(CreateProductDto dto)
        {
            try
            {
                // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userId =  (await _userManager.GetUserAsync(User)).Id;
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    IsActive = true,
                    IsDeleted = false,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    Condition = dto.Condition,
                    Brand = dto.Brand,
                    SellerId = userId,
                    CategoryId = dto.CategoryId,
                };

                await _productRepository.CreateProductAsync(product);

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
                    Message = "Product created successfully",
                    Data = productDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the product."
                });
            }
        }
        
        
        // PUT: api/product/5
        [HttpPut("{id}")]
        [ Authorize]
        public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(int id, UpdateProductDto dto)
        {
            try
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

                // Check if the current user is the seller of the product
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (product.SellerId != userId)
                {
                    return Unauthorized(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "You are not authorized to update this product"
                    });
                }

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.Stock = dto.Stock;
                product.Condition = dto.Condition;
                product.Brand = dto.Brand;
                product.CategoryId = dto.CategoryId;

                await _productRepository.UpdateProductAsync(product);

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
                    Message = "Product updated successfully",
                    Data = productDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the product."
                });
            }
        }
        
        
        // DELETE: api/product/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductDto>>> DeleteProduct(int id)
        {
            try
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

                // Check if the current user is the seller of the product
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (product.SellerId != userId)
                {
                    return Unauthorized(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "You are not authorized to delete this product"
                    });
                }
                
                if(product.IsDeleted)
                {
                    return BadRequest(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Product already deleted"
                    });
                }

                await _productRepository.SoftDeleteProductAsync(product);

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
                    CategoryName = product.Category?.Name,
                };

                return Ok(new ApiResponse<ProductDto>
                {
                    Success = true,
                    Message = "Product soft deleted successfully",
                    Data = productDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = $"An error occurred while trying to soft delete the product. {ex}"
                });
            }
        }
        
        
        // Deactivate a product
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductDto>>> DeactivateProduct(int id)
        {
            try
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
                
                if(!product.IsActive)
                {
                    return BadRequest(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Product already active"
                    });
                }

                // Check if the current user is the seller of the product
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (product.SellerId != userId)
                {
                    return Unauthorized(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "You are not authorized to deactivate this product"
                    });
                }

                if (!product.IsActive)
                {
                    return BadRequest(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Product already deactivated"
                    });
                }

                product.IsActive = false;
                await _productRepository.UpdateProductAsync(product);

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
                    CategoryName = product.Category?.Name,
                };

                return Ok(new ApiResponse<ProductDto>
                {
                    Success = true,
                    Message = "Product deactivated successfully",
                    Data = productDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = $"An error occurred while trying to deactivate the product. {ex}"
                });
            }
        }
        
        // Activate a product
        [HttpPut("{id}/activate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductDto>>> ActivateProduct(int id)
        {
            try
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
                
                if(product.IsActive)
                {
                    return BadRequest(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Product already active"
                    });
                }

                // Check if the current user is the seller of the product
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (product.SellerId != userId)
                {
                    return Unauthorized(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "You are not authorized to activate this product"
                    });
                }

                if (product.IsActive)
                {
                    return BadRequest(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Product already active"
                    });
                }

                product.IsActive = true;
                await _productRepository.UpdateProductAsync(product);

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
                    CategoryName = product.Category?.Name,
                };

                return Ok(new ApiResponse<ProductDto>
                {
                    Success = true,
                    Message = "Product activated successfully",
                    Data = productDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = $"An error occurred while trying to activate the product. {ex}"
                });
            }
        }
       
    }

    
}
