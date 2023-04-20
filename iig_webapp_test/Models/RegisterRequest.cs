﻿using System.ComponentModel.DataAnnotations;

namespace iig_webapp_test.Models
{
    public class RegisterRequest
    {
        [Required]
        [MinLength(6)]
        [MaxLength(12)]
        [RegularExpression("^[A-Za-z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        public string Username { get; set; }
        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; }
        [MaxLength(60)]
        public string LastName { get; set; }
        [Required]
        public string ProfileImage { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
