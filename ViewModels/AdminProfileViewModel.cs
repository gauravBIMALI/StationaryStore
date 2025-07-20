using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class AdminProfileViewModel
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string? ProfileImagePath { get; set; }
        [Display(Name = "Upload Profile Image")]
        // This handles the upload
        public IFormFile? ProfileImage { get; set; }
    }
}
