using System.ComponentModel.DataAnnotations;

namespace AuthAlbiWebSchool.Models;
public class CreateProductDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; }
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Description { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Stock { get; set; }
    [Required]
    public string Condition { get; set; }
    [Required]
    public string Brand { get; set; }
    public int CategoryId { get; set; } 
}   