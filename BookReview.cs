namespace ReadZone.Models
{
    public class BookReview
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string ReviewerName { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
