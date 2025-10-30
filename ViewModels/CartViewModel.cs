namespace ClzProject.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        public decimal TotalAmount => CartItems.Sum(item => item.SubTotal);

        public int TotalItems => CartItems.Sum(item => item.Quantity);

        public int UniqueProducts => CartItems.Count;
    }
}
