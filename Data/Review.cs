namespace AuthAlbiWebSchool.Data;

public class Review : BaseEntity
{
    public double Rating { get; set; }
    public string Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    
    //-----------------Relationships-----------------
    
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    
    
}