using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        //[Required(ErrorMessage = "Business Name is required.")]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Business Type is required.")]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "PAN (Base64)")]
        public string PANBase64 { get; set; } = string.Empty;

        [Display(Name = "Verified ID (Base64)")]
        public string VerifiedIDBase64 { get; set; } = string.Empty;

        // These properties are not mapped to database - used for file uploads
        [NotMapped]
        [Display(Name = "PAN Document")]
        public IFormFile? PANFile { get; set; }

        [NotMapped]
        [Display(Name = "Verified ID Document")]
        public IFormFile? VerifiedIDFile { get; set; }
    }
}