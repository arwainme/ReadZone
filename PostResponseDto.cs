namespace ReadZone.DTO
{
    public class PostResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
    }

}
