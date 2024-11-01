namespace AuthAlbiWebSchool.Data;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string Condition { get; set; }
    public bool IsActive { get; set; }
    public string Brand { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

    // Foreign keys
    public string SellerId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public virtual User Seller { get; set; }
    public virtual Category Category { get; set; }
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}