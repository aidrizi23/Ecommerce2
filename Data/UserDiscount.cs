namespace AuthAlbiWebSchool.Data;

public class UserDiscount
{
    public string UserId { get; set; }
    public virtual User User { get; set; }
    
    public int DiscountId { get; set; }
    public virtual Discount Discount { get; set; }
}