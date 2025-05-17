using System.ComponentModel.DataAnnotations;

namespace ReadZone.DTO
{
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; }

    }
}
