using Microsoft.AspNetCore.Identity;

namespace AuthAlbiWebSchool.Data;

public class User : IdentityUser<string>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    
    
}