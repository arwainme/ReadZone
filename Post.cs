using ReadZone.Models;

namespace ReadZone.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // علاقة بالمستخدم
        public int UserId { get; set; }
        public User? User { get; set; }

        // عدد اللايكس
        public int LikeCount { get; set; } = 0;

        // قائمة المستخدمين اللي عملوا لايك (اختياري)
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }

}


