using System.ComponentModel.DataAnnotations;

namespace ReadZone.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
