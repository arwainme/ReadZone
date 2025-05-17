namespace ReadZone.Models
{
    public class SpecialOffer
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
