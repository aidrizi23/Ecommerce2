using AuthAlbiWebSchool.Data;
using AuthAlbiWebSchool.Repositories;
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
        
        
    }
}
