using System.ComponentModel.DataAnnotations;

namespace UserRoles.ViewModels
{
    public class SellerRegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        [Display(Name = "Age")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Business name is required")]
        [StringLength(100, ErrorMessage = "Business name cannot exceed 100 characters")]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Business type is required")]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; } // Changed to string to match dropdown values

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password must be atleast 4 characters")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
        //    ErrorMessage = "Password must contain uppercase, lowercase and number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}