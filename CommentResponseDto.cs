namespace ReadZone.DTO
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
