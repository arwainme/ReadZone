using System.ComponentModel.DataAnnotations;

namespace ReadZone.DTO
{
    // DTO/UpdateProfileDto.cs
    public class UpdateProfileDto
    {
        public string? Username { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }

}
