using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UserRoles.Models
{
    public class Users : IdentityUser
    {
        [Required, MaxLength(100)]
        public string? FullName { get; set; }
        public bool EmailConfirmed { get; set; } = true;

        [MaxLength(100)]
        public string? BusinessName { get; set; }

        [MaxLength(50)]
        public string? BusinessType { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        public int? Age { get; set; }
        public string? ProfileImage { get; set; }
    }
}