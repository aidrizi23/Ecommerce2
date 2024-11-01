namespace AuthAlbiWebSchool.Data;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    // Foreign keys
    public int CartId { get; set; }
    public int ProductId { get; set; }

    // Navigation properties
    public virtual Cart Cart { get; set; }
    public virtual Product Product { get; set; }
}
