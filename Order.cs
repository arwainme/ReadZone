namespace ReadZone.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BookIds { get; set; } // مثال: "123,456"
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public string? PromoCode { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
