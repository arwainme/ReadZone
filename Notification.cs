using ReadZone.Models;

public class Notification
{
    public int Id { get; set; }
    public string Type { get; set; }

    public int RecipientUserId { get; set; }
    public User RecipientUser { get; set; }

    public int SourceUserId { get; set; }
    public User SourceUser { get; set; }

    public int? PostId { get; set; }
    public Post? Post { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

