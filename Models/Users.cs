using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UserRoles.Models
{
    public class Users : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        // Seller-specific fields (nullable)
        [MaxLength(100)]
        public string? BusinessName { get; set; }

        [MaxLength(50)]
        public string? BusinessType { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        public int? Age { get; set; }

    }
}