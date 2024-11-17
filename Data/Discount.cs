namespace AuthAlbiWebSchool.Data;

public abstract class Discount
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool IsActive { get; set; }
    
    
    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();
    public virtual ICollection<UserDiscount> UserDiscounts { get; set; } = new List<UserDiscount>();
    
    public bool IsValid(DateTime currentDate)
    {
        return currentDate >= StartDate && currentDate <= EndDate && IsActive;
    }
    


}