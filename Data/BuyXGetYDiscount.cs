namespace AuthAlbiWebSchool.Data;

public class BuyXGetYDiscount : Discount
{
    public int RequiredQuantity { get; set; }
    public int FreeQuantity { get; set; }
    
    public int CalculateFreeItems(int quantity)
    {
        return (quantity / RequiredQuantity) * FreeQuantity;
    }
    
    public decimal CalculateDiscount(int quantity, decimal unitPrice)
    {
        if (quantity >= RequiredQuantity)
        {
            int freeItems = (quantity / RequiredQuantity) * FreeQuantity;
            return freeItems * unitPrice;  // Total value of free items
        }
        return 0;
    }

}