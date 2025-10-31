using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class CheckoutViewModel
    {
        // Cart Summary
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; } = 60;
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }

        // Delivery Information
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string DeliveryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string DeliveryPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(50)]
        [Display(Name = "City")]
        public string DeliveryCity { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "State/Province")]
        public string? DeliveryState { get; set; }

        [StringLength(500)]
        [Display(Name = "Delivery Note (Optional)")]
        public string? DeliveryNote { get; set; }

        // Payment Method
        [Required(ErrorMessage = "Please select a payment method")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "COD";

    }
}
