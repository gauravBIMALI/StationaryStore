using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class SellerOrderViewModel
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        public int ProductId { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; } = string.Empty;

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        // Buyer Information
        [Display(Name = "Buyer Name")]
        public string BuyerName { get; set; } = string.Empty;

        [Display(Name = "Buyer Phone")]
        public string BuyerPhone { get; set; } = string.Empty;

        // Delivery Information
        [Display(Name = "Delivery Name")]
        public string DeliveryName { get; set; } = string.Empty;

        [Display(Name = "Delivery Phone")]
        public string DeliveryPhone { get; set; } = string.Empty;

        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Display(Name = "City")]
        public string DeliveryCity { get; set; } = string.Empty;

        public string? DeliveryState { get; set; }

        [Display(Name = "Delivery Note")]
        public string? DeliveryNote { get; set; }

        public string? ProductImage { get; set; }

    }
}
