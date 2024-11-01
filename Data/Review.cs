namespace AuthAlbiWebSchool.Data;

public class Review : BaseEntity
{
    public double Rating { get; set; }
    public string Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Foreign keys
    public int ProductId { get; set; }
    public string UserId { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual User User { get; set; }
}