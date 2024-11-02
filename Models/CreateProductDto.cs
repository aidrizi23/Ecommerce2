namespace AuthAlbiWebSchool.Models;
public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string Condition { get; set; }
    public string Brand { get; set; }
    public int CategoryId { get; set; } // If applicable
}   