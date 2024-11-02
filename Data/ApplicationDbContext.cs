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

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User configurations
            builder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product configurations
            builder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(u => u.ProductsForSale)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review configurations
            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order configurations
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductOrder>()
                .HasOne(po => po.Order)
                .WithMany(o => o.ProductOrders)
                .HasForeignKey(po => po.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductOrder>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOrders)
                .HasForeignKey(po => po.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add useful indexes
            builder.Entity<Product>()
                .HasIndex(p => p.SellerId);

            builder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            builder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId });

            builder.Entity<ProductOrder>()
                .HasIndex(po => new { po.OrderId, po.ProductId });


            // create default roles
            builder.Entity<Role>().HasData(
                new Role
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new Role
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER"
                },
                new Role
                {
                    Id = "3",
                    Name = "Seller",
                    NormalizedName = "SELLER"
                }

            );

            // create default user
            var hasher = new PasswordHasher<User>();
            builder.Entity<User>().HasData(
                new User
                {
                    Id = "1",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "admin@admin.com",
                    UserName = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    NormalizedUserName = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "albiidrizi27"),


                });

            // add the default user to the Admin role
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "1",
                    UserId = "1"
                }
            );

           // static categories
            builder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Electronics"
                },
                new Category
                {
                    Id = 2,
                    Name = "Clothing"
                },
                new Category
                {
                    Id = 3,
                    Name = "Books"
                }
            );


        }
}
