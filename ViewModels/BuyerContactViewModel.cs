using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class BuyerContactViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business name is required")]
        [MaxLength(200)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business type is required")]
        [MaxLength(100)]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Citizenship document is required")]
        [Display(Name = "Citizenship Document")]
        public IFormFile? CitizenshipDocument { get; set; }

        [Required(ErrorMessage = "PAN document is required")]
        [Display(Name = "PAN Document")]
        public IFormFile? PANDocument { get; set; }
    }
}