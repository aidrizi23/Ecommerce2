namespace AuthAlbiWebSchool.Models;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }
    public double Subtotal => UnitPrice * Quantity;
}