﻿using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Account.Models.Views
{
    public class ProfileForm
    {
        [Required]
        //[EmailAddress]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool? RememberMe { get; set; }

        public string? Response { get; set; }
    }
}
