namespace AuthAlbiWebSchool.Data;

public class PercentageDiscount : Discount
{
    public decimal PercentageOff { get; set; }

    public decimal CalculateDiscount(decimal originalPrice)
    {
        if (PercentageOff < 0 || PercentageOff > 100)
            throw new ArgumentException("PercentageOff must be between 0 and 100.");
        return originalPrice * (PercentageOff / 100);
    }
}