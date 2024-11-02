namespace AuthAlbiWebSchool.Models;

public class CartResponseDto
{
    public List<CartItemDto> Items { get; set; }
    public double Total { get; set; }
}