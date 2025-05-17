namespace ReadZone.Models
{
    public class AudioBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Narrator { get; set; }
        public TimeSpan Duration { get; set; }
        public string AudioUrl { get; set; }
        public string CoverImageUrl { get; set; }
    }
}
