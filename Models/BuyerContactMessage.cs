using System.ComponentModel.DataAnnotations;

namespace ClzProject.Models
{
    public class BuyerContactMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business name is required")]
        [MaxLength(200)]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business type is required")]
        [MaxLength(100)]
        public string BusinessType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Citizenship document is required")]
        public string CitizenshipDocumentPath { get; set; } = string.Empty;

        [Required(ErrorMessage = "PAN document is required")]
        public string PANDocumentPath { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public bool IsReplied { get; set; } = false;

        [MaxLength(1000)]
        public string? AdminReply { get; set; }

        public DateTime? RepliedAt { get; set; }

        public string? AdminUserId { get; set; }
    }
}