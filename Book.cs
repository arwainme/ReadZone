using System.ComponentModel.DataAnnotations.Schema;

namespace ReadZone.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [NotMapped]
        public List<Author> Authors { get; set; } = new(); // ✅ Add this

        [NotMapped]
        public List<string> Subjects { get; set; } = new();

        [NotMapped]
        public Dictionary<string, string>? Formats { get; set; }

        public string? CoverImageUrl => Formats?.FirstOrDefault(x => x.Key.Contains("image/jpeg")).Value;

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
