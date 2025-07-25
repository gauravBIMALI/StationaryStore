﻿using System.ComponentModel.DataAnnotations;

namespace UserRoles.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Password Must be 4 character long")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;

    }
}
