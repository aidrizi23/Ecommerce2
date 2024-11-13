namespace AuthAlbiWebSchool.Models.Reviews;

public class ReviewDto
{
    
    public int Id { get; set; }

    public double Rating { get; set; }
    public string Comment { get; set; }
    public string ReviewerName { get; set; }
    public string ProductName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}