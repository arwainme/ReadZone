using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadZone.Models
{
    public class UserLibraryBook
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        public int UserLibraryId { get; set; }

        [ForeignKey("UserLibraryId")]
        public UserLibrary UserLibrary { get; set; }

        public bool IsFavorite { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsDownloaded { get; set; }
    }
}

