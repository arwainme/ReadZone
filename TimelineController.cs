using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadZone.DTO;
using ReadZone.Models;
using System.Security.Claims;
using ReadZone.Data;
using Microsoft.EntityFrameworkCore;

namespace ReadZone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TimelineController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // ✅ إضافة بوست
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPost(CreatePostDto dto)
        {
            var userId = GetUserId();

            var post = new Post
            {
                Content = dto.Content,
                UserId = userId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok("Post created");
        }

        // ✅ عرض كل البوستات (تايملاين عام)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    Username = p.User!.Username,
                    ProfileImageUrl = p.User.ProfileImageUrl ?? "",
                    CreatedAt = p.CreatedAt,
                    LikeCount = p.LikeCount
                })
                .ToListAsync();

            return Ok(posts);
        }

        // ✅ لايك / إزالة لايك مع إنشاء إشعار
        [Authorize]
        [HttpPost("{postId}/like")]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = GetUserId();

            var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) return NotFound();

            var existingLike = await _context.PostLikes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                // حذف اللايك
                _context.PostLikes.Remove(existingLike);
                post.LikeCount--;
            }
            else
            {
                // إضافة لايك
                var like = new PostLike
                {
                    PostId = postId,
                    UserId = userId
                };

                _context.PostLikes.Add(like);
                post.LikeCount++;

                // إنشاء إشعار إذا كان صاحب البوست غير المستخدم الحالي
                if (post.UserId != userId)
                {
                    var notification = new Notification
                    {
                        Type = "Like",
                        RecipientUserId = post.UserId,
                        SourceUserId = userId,
                        PostId = postId,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Notifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Like toggled");
        }

        // ✅ حذف بوست
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.UserId != userId)
                return Unauthorized("You can only delete your own posts");

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok("Post deleted");
        }

        // ✅ إضافة كومنت مع إنشاء إشعار
        [Authorize]
        [HttpPost("{postId}/comment")]
        public async Task<IActionResult> AddComment(int postId, AddCommentDto dto)
        {
            var userId = GetUserId();

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return NotFound("Post not found");

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Text = dto.Text
            };

            _context.Comments.Add(comment);

            // إنشاء إشعار إذا كان صاحب البوست غير المستخدم الحالي
            if (post.UserId != userId)
            {
                var notification = new Notification
                {
                    Type = "Comment",
                    RecipientUserId = post.UserId,
                    SourceUserId = userId,
                    PostId = postId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return Ok("Comment added");
        }

        // ✅ عرض التعليقات على بوست معين
        [AllowAnonymous]
        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetComments(int postId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    Username = c.User!.Username,
                    ProfileImageUrl = c.User.ProfileImageUrl ?? "",
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        // ✅ حذف تعليق
        [Authorize]
        [HttpDelete("comment/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = GetUserId();

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
                return NotFound();

            if (comment.UserId != userId)
                return Unauthorized("You can only delete your own comments");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment deleted");
        }

        // ———— إضافة هنا: جلب إشعارات المستخدم (مرتبة، مع بيانات مرسلة) ————
        [Authorize]
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetUserId();

            var notifications = await _context.Notifications
                .Include(n => n.SourceUser)
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    n.Id,
                    n.Type,
                    n.PostId,
                    SourceUsername = n.SourceUser.Username,
                    SourceProfileImageUrl = n.SourceUser.ProfileImageUrl ?? "",
                    n.IsRead,
                    n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        // ———— تمييز إشعار كمقروء ————
        [Authorize]
        [HttpPost("notifications/{notificationId}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            var userId = GetUserId();

            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientUserId == userId);
            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok("Notification marked as read");
        }
    }
}
