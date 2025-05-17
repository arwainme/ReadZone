using ReadZone.Models;

namespace ReadZone.Models
{
    public class BookRating
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; } // Or int if using integer IDs
        public double Value { get; set; } // التقييم من 1 لـ 5
        public DateTime RatedAt { get; set; } = DateTime.UtcNow;
        
    }
}
