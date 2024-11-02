namespace AuthAlbiWebSchool.Models;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string Condition { get; set; }
    public string Brand { get; set; }
    public string SellerName { get; set; }
    public string CategoryName { get; set; }
}