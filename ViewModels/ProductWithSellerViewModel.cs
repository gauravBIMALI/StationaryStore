﻿using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class ProductWithSellerViewModel
    {
        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string ProductDescription { get; set; } = string.Empty;

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Quantity")]
        public int ProductQuantity { get; set; }

        [Display(Name = "Category")]
        public string CategoryType { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated Date")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public string SellerId { get; set; } = string.Empty;

        [Display(Name = "Seller Name")]
        public string SellerName { get; set; } = string.Empty;

        [Display(Name = "Seller Email")]
        public string SellerEmail { get; set; } = string.Empty;

        [Display(Name = "Business Name")]
        public string SellerBusinessName { get; set; } = string.Empty;
    }
}