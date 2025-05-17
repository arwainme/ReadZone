namespace ReadZone.Models
{
    public class OrderDto
    {
        public int UserId { get; set; }
        public List<int> BookIds { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public string? PromoCode { get; set; }
        public decimal TotalCost { get; set; }
    }
}
