using System.ComponentModel.DataAnnotations;

namespace ClzProject.Models
{
    public class AdminContact
    {
        [Key]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business Name is required.")]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business Type is required.")]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "PAN document is required.")]
        [Display(Name = "PAN (Base64)")]
        public string PANBase64 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verified ID is required.")]
        [Display(Name = "Verified ID (Base64)")]
        public string VerifiedIDBase64 { get; set; } = string.Empty;
    }
}
