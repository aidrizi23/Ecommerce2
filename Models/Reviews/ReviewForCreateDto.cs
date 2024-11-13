namespace AuthAlbiWebSchool.Models.Reviews;

public class ReviewForCreateDto
{
    public double Rating { get; set; }
    public string Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    // public string UserId { get; set; }
    public int ProductId { get; set; }
    
}