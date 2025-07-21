using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class SellerProfileViewModel
    {

        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessType { get; set; }

        [Display(Name = "Profile Image (Base64)")]
        public string? ProfileImageBase64 { get; set; }  // This will store the Base64 string

        public string? ProfileImagePath { get; set; }
        [Display(Name = "Upload Profile Image")]
        // This handles the upload
        public IFormFile? ProfileImage { get; set; }
    }
}