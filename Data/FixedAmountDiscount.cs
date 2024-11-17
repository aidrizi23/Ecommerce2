namespace AuthAlbiWebSchool.Data;


public class FixedAmountDiscount : Discount
{
    public decimal AmountOff { get; set; }

    public decimal CalculateDiscount(decimal originalPrice)
    {
        decimal discountedPrice = originalPrice - AmountOff;
        return discountedPrice < 0 ? 0 : AmountOff;  // Ensure the discount is not negative
    }

}