namespace AuthAlbiWebSchool.Data;

public class ProductOrder
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    // Foreign keys
    public int OrderId { get; set; }
    public int? ProductId { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
}
