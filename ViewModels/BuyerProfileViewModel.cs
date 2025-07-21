using System.ComponentModel.DataAnnotations;
namespace ClzProject.ViewModels
{
    public class BuyerProfileViewModel
    {
        // This class represents the view model for a buyer's profile.


        //[Key]
        //public int BuyerId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [Display(Name = "Full Name")]
        [DataType(DataType.Text)]

        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email Address")]

        public string Email { get; set; } = null!;



        [Display(Name = "Profile Image (Base64)")]
        public string? ProfileImageBase64 { get; set; }  // This will store the Base64 string

        public string? ProfileImagePath { get; set; }
        [Display(Name = "Upload Profile Image")]
        // This handles the upload
        public IFormFile? ProfileImage { get; set; }

    }
}
