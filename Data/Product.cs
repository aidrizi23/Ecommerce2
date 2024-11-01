namespace AuthAlbiWebSchool.Data;

public class Product : BaseEntity
{
    public string Name { get; set; }    
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Condition { get; set; }
    public bool IsActive { get; set; }
    public string Brand { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    
    
    //-----------------Relationships-----------------
    
    public int SellerId { get; set; }
    public virtual User Seller { get; set; }
    
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }
    
    
    
    
}