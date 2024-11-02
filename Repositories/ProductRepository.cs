using AuthAlbiWebSchool.Data;
using AuthAlbiWebSchool.Models;
using AuthAlbiWebSchool.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthAlbiWebSchool.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // method to create a new product
    public async Task<Product> CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }
    
    // method to update a product
    public async Task<Product> UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }
    
    
    // method to delete a product
    public async Task<Product> DeleteProductAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return product;
    }
    
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive && p.Stock > 0)
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Include(x => x.Reviews)
            .ToListAsync();
    }

    public async Task<PaginatedList<Product>> GetAllPaginatedProductsAsync()
    {
        return await PaginatedList<Product>.CreateAsync(_context.Products
            .Where(p => p.IsActive && p.Stock > 0)
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Include(x => x.Reviews), 1, 10);
        
    }
    
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Where(p => p.Id == id)
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync();
    }

    public async Task AddToCartAsync(string userId, int productId, int quantity)
    {
        // get the product and ensure it is active
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
        
        if(product == null)
            throw new InvalidOperationException("Product not found or inactive");
        
        if(product.Stock < quantity)
            throw new InvalidOperationException("Not enough stock");
        
        // now get the cart where will be able to add the product
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        // if the user does not have a cart, create one
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }
        
        // will check if the product is already in the cart
        var existingItem = cart.CartItems
            .FirstOrDefault(ci => ci.ProductId == productId);

        if (existingItem != null)
        {
            // first will check if the new quantity is available
            if (product.Stock < existingItem.Quantity + quantity)
                throw new InvalidOperationException("Not enough stock");
            
            existingItem.Quantity += quantity;
            
        }
        else
        {
            // if the product is not in the cart, add it
            cart.CartItems.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            });
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<OrderResult> BuyNowAsync(string userId, int productId, int quantity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // get the product and ensure it is active
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive == true);

            if (product == null)
                throw new InvalidOperationException("Product not found or inactive");

            if (product.Stock < quantity)
                throw new InvalidOperationException("Not enough stock available");

            // now let's create an order
            var order = new Order()
            {
                UserId = userId,
                OrderDate = DateTimeOffset.UtcNow,
                Status = "Pending",
                TotalAmount = product.Price * quantity
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var productOrder = new ProductOrder
            {
                OrderId = order.Id,
                ProductId = productId,
                Quantity = quantity
            };

            _context.ProductOrders.Add(productOrder);
            product.Stock -= quantity;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new OrderResult
            {
                Success = true,
                OrderId = order.Id,
                Message = "Order placed successfully"
            };

        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new OrderResult
            {
                Success = false,
                Message = ex.Message
            };
        }



    }

    public async Task<OrderResult> CheckoutCartAsync(string userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // first we will retrieve the cart of the current user.
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            
            // check if the cart is empty
            if (cart?.CartItems == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty");
            
            // verifying if there is enough stock for all items
            foreach (var item in cart.CartItems)
            {
                if (item.Product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Not enough stock for product: {item.Product.Name}");
            }
            
             // creating a new order
             var order = new Order()
             {
                 TotalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                 Status = "Pending",
                 UserId = userId,
                 OrderDate = DateTimeOffset.UtcNow,
             };
             _context.Orders.Add(order);
             await _context.SaveChangesAsync(); 
             
             // now create a productOrder for each order 
             foreach (var item in cart.CartItems)
             {
                 var productOrder = new ProductOrder
                 {
                     OrderId = order.Id,
                     ProductId = item.ProductId,
                     Quantity = item.Quantity
                 };
                 _context.ProductOrders.Add(productOrder);

                 // Update product stock
                 item.Product.Stock -= item.Quantity;
             }
             
             // Clear the cart
             _context.CartItems.RemoveRange(cart.CartItems); 
             await _context.SaveChangesAsync();
             await transaction.CommitAsync();
            
             
             return new OrderResult
             {
                 Success = true,
                 OrderId = order.Id,
                 Message = "Order placed successfully"
             };
             
             
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new OrderResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
    
    public async Task<CartResponseDto> GetCartAsync(string userId)
    {
        // get the cart of the user
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            return new CartResponseDto 
            { 
                Items = new List<CartItemDto>(),
                Total = 0 
            };
        }

        var items = cart.CartItems.Select(ci => new CartItemDto
        {
            ProductId = ci.ProductId,
            ProductName = ci.Product.Name,
            UnitPrice = ci.Product.Price,
            Quantity = ci.Quantity
        }).ToList();

        return new CartResponseDto
        {
            Items = items,
            Total = items.Sum(i => i.Subtotal)
        };
    }


}


public interface IProductRepository
{
    Task<List<Product>> GetAllProductsAsync();
    Task<PaginatedList<Product>> GetAllPaginatedProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task AddToCartAsync(string userId, int productId, int quantity);
    Task<OrderResult> BuyNowAsync(string userId, int productId, int quantity);
    Task<OrderResult> CheckoutCartAsync(string userId);
    Task<CartResponseDto> GetCartAsync(string userId);
    
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<Product> DeleteProductAsync(Product product);
    
    
}