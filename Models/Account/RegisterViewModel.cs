using System.ComponentModel.DataAnnotations;

namespace AuthAlbiWebSchool.Models.Account;

public class RegisterViewModel
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public required string FirstName { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public required string LastName { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
    
    
    public bool RememberMe { get; set; }
}