using System.ComponentModel.DataAnnotations;

namespace AuthAlbiWebSchool.Models.Account;

public class UserUpdateViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    [ EmailAddress ]
    public string Email { get; set; }
}