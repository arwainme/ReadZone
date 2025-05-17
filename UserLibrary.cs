using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadZone.Models
{
    public class UserLibrary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User user { get; set; }

        public ICollection<UserLibraryBook> LibraryBooks { get; set; }
    }
}

