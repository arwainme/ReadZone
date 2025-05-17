namespace ReadZone.Models
{
    public class Note
    {
        public int Id { get; set; } // معرف الملاحظة
        public int UserId { get; set; } // معرف المستخدم (ربط الملاحظة بالمستخدم)
        public string Content { get; set; } // محتوى الملاحظة
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
