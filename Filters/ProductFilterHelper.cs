using AuthAlbiWebSchool.Data.Migrations;

namespace AuthAlbiWebSchool.Filters;

public class ProductFilterHelper
{
    public string? Name { get; set; } 
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public int? CategoryId { get; set; }
    public double? Rating { get; set; }
}