namespace ReadZone.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Only needed if you're saving locally
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
