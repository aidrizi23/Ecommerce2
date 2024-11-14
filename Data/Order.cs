namespace AuthAlbiWebSchool.Data;

public class Order
{
    public int Id { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public double TotalAmount { get; set; }
    public string Status { get; set; }

    // Foreign key
    public string? UserId { get; set; }
    public virtual User User { get; set; }
    
    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}