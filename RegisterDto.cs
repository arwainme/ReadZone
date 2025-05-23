﻿using System.ComponentModel.DataAnnotations;

namespace ReadZone.DTO
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        
        public string? ProfileImageUrl { get; set; }

        public DateTime BirthDate { get; set; }
    }

}
