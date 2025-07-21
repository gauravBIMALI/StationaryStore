using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class SellerProfileViewModel
    {

        public string? Name { get; set; }


        public string? Email { get; set; }
        public int Age { get; set; }
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public string BusinessType { get; set; } = null!;

        [Display(Name = "Profile Image (Base64)")]
        public string? ProfileImageBase64 { get; set; }  // This will store the Base64 string

        public string? ProfileImagePath { get; set; }
        [Display(Name = "Upload Profile Image")]
        // This handles the upload
        public IFormFile? ProfileImage { get; set; }
    }
}