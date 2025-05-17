using System.ComponentModel.DataAnnotations;

namespace ReadZone.Models
{
    public class User 
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }  
        public string? Role { get; set; } = "User";

        public DateTime BirthDate { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? ResetOtp { get; set; }

        public DateTime? OtpExpiryTime { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<UserFavoriteBook> FavoriteBooks { get; set; } = new List<UserFavoriteBook>();
        public ICollection<UserBookmarkBook> BookmarkedBooks { get; set; } = new List<UserBookmarkBook>();
        public ICollection<UserDownloadedBook> DownloadedBooks { get; set; } = new List<UserDownloadedBook>();


    }

}
