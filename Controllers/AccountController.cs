    using AuthAlbiWebSchool.Data;
    using AuthAlbiWebSchool.Models;
    using AuthAlbiWebSchool.Models.Account;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    namespace AuthAlbiWebSchool.Controllers
    {
        [Route("api/account")]
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
                
                // add the user to the default role (User)
                await _userManager.AddToRoleAsync(user, "User");
                
                // sign in the user
                await _signInManager.SignInAsync(user, model.RememberMe);

                var response = new ApiResponse<User>()
                {
                    Success = true,
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
                
            
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if(user != null && user.LockoutEnd > DateTimeOffset.Now && user.AccountDeletionRequested)
                    await RecoverAccount(user);
                
                if (user != null && await _userManager.IsLockedOutAsync(user))
                {
                    ModelState.AddModelError("Lockout", "User account locked out");
                    return BadRequest(ModelState);
                }
                
                
                
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("Login", "Invalid login attempt");
                    Console.WriteLine(result.ToString());
                    
                    return BadRequest(ModelState);
                }
                
                // var user = await _userManager.FindByEmailAsync(model.Email);
                
                var response = new ApiResponse<User>()
                {
                    Success = true,
                    Message = "User logged-in successfully",
                    Data = user
                };
                
                return Ok(response);
            }
            
            
            [Authorize]
            [HttpDelete("delete-account")]
            public async Task<IActionResult> DeleteAccount(string password)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
        
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.CheckPasswordAsync(user, password);

                if (!result)
                {
                    ModelState.AddModelError("Password", "Password is incorrect");
                    return BadRequest(ModelState);
                }

                user.LockoutEnd = DateTimeOffset.Now.AddMinutes(2);
                user.AccountDeletionRequested = true;
                await _userManager.UpdateAsync(user);
                await _signInManager.SignOutAsync();

                var response = new ApiResponse<string>()
                {
                    Success = true,
                    Message = "Account scheduled for deletion",
                    Data = null
                };

                return Ok(response);
            }

            private async Task RecoverAccount(User user)
            {
                user.LockoutEnd = null;
                user.AccountDeletionRequested = false;
                await _userManager.UpdateAsync(user);
        
            }
            
            
            
            
            [HttpPost("logout")]
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            
            // method to make the current user a seller
            [HttpPost("make-seller")]
            public async Task<IActionResult> MakeSeller()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError("User", "User not found");
                    return BadRequest(ModelState);
                }
                
                var result = await _userManager.AddToRoleAsync(user, "Seller");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                
                var response = new ApiResponse<User>()
                {
                    Success = true,
                    Message = "User is now a seller",
                    Data = user
                };
                
                return Ok(response);
            }
            
            
            // making a getCurrentUser method jsut for testing
            [HttpGet("current-user")]
            public async Task<IActionResult> GetCurrentUser()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError("User", "User not found");
                    return BadRequest(ModelState);
                }
                
                var response = new ApiResponse<User>()
                {
                    Success = true,
                    Message = "User found",
                    Data = user
                };
                
                return Ok(response);
            }
            
            // method to change the password
            [HttpPost("change-password")]
            [Authorize]
            public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError("User", "User not found");
                    return BadRequest(ModelState);
                }
                
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                
                var response = new ApiResponse<User>()
                {
                    Success = true,
                    Message = "Password changed successfully",
                    Data = user
                };
                
                return Ok(response);
            }
            
            // method for user update
            [HttpPut("update")]
            [Authorize]
            public async Task<IActionResult> UpdateUser([FromBody] UserUpdateViewModel model)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError("User", "User not found");
                    return BadRequest(ModelState);
                }
                
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                
                
                // updating the email if it has changed
                if (user.Email != model.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "Email already exists");
                        return BadRequest(ModelState);
                    }
                    
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.NormalizedEmail = model.Email.ToUpper();
                    user.NormalizedUserName = model.Email.ToUpper();
                }
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                
                var response = new ApiResponse<User>()
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = user
                };
                
                return Ok(response);
            }
            
            
            
            
            // method to deactivate the account
            [HttpDelete("deactivate-account")]
            [Authorize]
            public async Task<IActionResult> DeactivateAccount()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError("User", "User not found");
                    return BadRequest(ModelState);
                }
                
                user.LockoutEnd = DateTimeOffset.MaxValue;
                await _userManager.UpdateAsync(user);
                await _signInManager.SignOutAsync();
                
                var response = new ApiResponse<string>()
                {
                    Success = true,
                    Message = "Account deactivated",
                    Data = null
                };
                
                return Ok(response);
            }
        }
    }
