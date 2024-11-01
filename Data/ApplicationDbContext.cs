using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthAlbiWebSchool.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role,string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Devices and gadgets" },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel and accessories" },
            new Category { Id = 3, Name = "Home Appliances", Description = "Devices for home use" },
            new Category { Id = 4, Name = "Books", Description = "Literature and publications" }
        );

    }
    
    
}
