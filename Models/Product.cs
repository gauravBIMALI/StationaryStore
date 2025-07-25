﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClzProject.Models
{
    [Table("Product")] // DB Table Name
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string ProductDescription { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")] // Specify decimal precision
        public decimal ProductPrice { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be zero or greater.")]
        public int ProductQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        //[Required]
        [Display(Name = "Image ")]
        public string Image { get; set; } = string.Empty; // Store base64 string
        [Required]
        public string CategoryType { get; set; } = string.Empty;

        public string SellerId { get; set; } = string.Empty;

        //public string? SellerId { get; internal set; }
        // FK to Identity User (Seller)
        //public string SellerId { get; set; } = string.Empty;
        //public Users Seller { get; set; }
    }
}
