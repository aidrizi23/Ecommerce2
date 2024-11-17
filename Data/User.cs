using Microsoft.AspNetCore.Identity;

namespace AuthAlbiWebSchool.Data;

public class User : IdentityUser<string>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    // Navigation properties
    public virtual Cart Cart { get; set; }
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Product> ProductsForSale { get; set; } = new List<Product>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    
    // field for account deletion request
    public bool AccountDeletionRequested { get; set; } = false;
    
    public virtual ICollection<UserDiscount> UserDiscounts { get; set; } = new List<UserDiscount>();
    public bool HasDiscounts => UserDiscounts.Any();
}