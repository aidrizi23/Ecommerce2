namespace AuthAlbiWebSchool.Data;

public class ProductDiscount
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    
    public int DiscountId { get; set; }
    public virtual Discount Discount { get; set; }
    
}