    using AuthAlbiWebSchool.Data;
    using AuthAlbiWebSchool.Models;
    using AuthAlbiWebSchool.Models.Account;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    namespace AuthAlbiWebSchool.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AccountController : ControllerBase
        {
            private readonly UserManager<User> _userManager;
            private readonly SignInManager<User> _signInManager;
            
            public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
            }
            
            
            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
            {
                
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                
                //check if the passwords match
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                    return BadRequest(ModelState);
                }
             
                
                // create an instance of a new user
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    NormalizedUserName = model.Email.ToUpper(),
                    EmailConfirmed = true,
                    
                };
                
                // check if the user exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return BadRequest(ModelState);
                }
                
                // create the user
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                
                // sign in the user
                await _signInManager.SignInAsync(user, model.RememberMe);

                var response = new Response()
                {
                    IsSuccess = true,
                    Message = "User created successfully",
                    Data = user
                };
                
                return Ok(response); // return user just for testing.
                
            }
            
            
            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginViewModel model)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("Login", "Invalid login attempt");
                    return BadRequest(ModelState);
                }
                
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                var response = new Response()
                {
                    IsSuccess = true,
                    Message = "User logged in successfully",
                    Data = user
                };
                
                return Ok(response);
            }
        }
    }
