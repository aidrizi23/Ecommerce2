    namespace AuthAlbiWebSchool.Data;

    public class Cart
    {
        public int Id { get; set; }
        
        // Foreign key
        public string UserId { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }