using ClzProject.Models;
using ClzProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using UserRoles.Data;
using UserRoles.Models;
using UserRoles.ViewModels;

namespace UserRoles.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<Users> userManager)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string categoryType)
        {
            // Get all active categories for dropdown
            var categories = await _context.Category.ToListAsync();

            // Filter products by category type if selected
            var productsQuery = _context.Product
                .Where(p => p.ProductQuantity > 0);

            if (!string.IsNullOrEmpty(categoryType))
            {
                productsQuery = productsQuery.Where(p => p.CategoryType == categoryType);
            }

            var products = await productsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    ProductQuantity = p.ProductQuantity,
                    CategoryType = p.CategoryType,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryType = categoryType;
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var product = await _context.Product
                .Where(p => p.ProductID == id)
                .Select(p => new { p.Image })
                .FirstOrDefaultAsync();

            if (product?.Image == null)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true, image = product.Image });
        }

        // GET: Home/ProductDetails/5
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Product
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<IActionResult> Category(string categoryType)
        {
            var products = await _context.Product
                .Where(p => p.CategoryType == categoryType && p.ProductQuantity > 0)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    ProductQuantity = p.ProductQuantity,
                    CategoryType = p.CategoryType,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            ViewBag.CategoryName = categoryType;
            return View("Index", products);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var products = await _context.Product
                .Where(p => (p.ProductName.Contains(searchTerm) ||
                           p.ProductDescription.Contains(searchTerm) ||
                           p.CategoryType.Contains(searchTerm)) &&
                           p.ProductQuantity > 0)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    ProductQuantity = p.ProductQuantity,
                    CategoryType = p.CategoryType,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            return View("Index", products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ProductReturn()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateSeller()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult UserDashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRelatedProducts(string categoryType, int excludeId, int count = 6)
        {
            try
            {
                var relatedProducts = await _context.Product
                    .Where(p => p.CategoryType == categoryType &&
                               p.ProductID != excludeId &&
                               p.ProductQuantity > 0)
                    .OrderBy(x => Guid.NewGuid())
                    .Take(count)
                    .Select(p => new
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        ProductPrice = p.ProductPrice,
                        CategoryType = p.CategoryType,
                        HasImage = !string.IsNullOrEmpty(p.Image)
                    })
                    .ToListAsync();

                return Json(new { success = true, products = relatedProducts });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Confirm()
        {
            return View();
        }

        public IActionResult Warranty()
        {
            return View();
        }

        //for comment
        [HttpGet]
        public async Task<IActionResult> GetProductComments(int productId)
        {
            try
            {
                var comments = await _context.ProductComments
                    .Where(c => c.ProductId == productId)
                    .Include(c => c.User)
                    .Include(c => c.Reply)
                        .ThenInclude(r => r.Seller)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDisplayViewModel
                    {
                        CommentId = c.CommentId,
                        CommentText = c.CommentText,
                        UserName = c.User.FullName ?? "Anonymous User",
                        CreatedAt = c.CreatedAt,
                        Reply = c.Reply != null ? new ReplyDisplayViewModel
                        {
                            ReplyId = c.Reply.ReplyId,
                            ReplyText = c.Reply.ReplyText,
                            SellerName = c.Reply.Seller.BusinessName ?? c.Reply.Seller.FullName ?? "Seller",
                            CreatedAt = c.Reply.CreatedAt
                        } : null
                    })
                    .ToListAsync();

                return Json(new { success = true, comments = comments });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to load comments" });
            }
        }

        // POST: Add a new comment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] AddCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid comment data" });
            }

            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Check if product exists
                var productExists = await _context.Product.AnyAsync(p => p.ProductID == model.ProductId);
                if (!productExists)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                var comment = new ProductComment
                {
                    ProductId = model.ProductId,
                    UserId = userId,
                    CommentText = model.CommentText,
                    CreatedAt = DateTime.Now
                };

                _context.ProductComments.Add(comment);
                await _context.SaveChangesAsync();

                var commentDisplay = new CommentDisplayViewModel
                {
                    CommentId = comment.CommentId,
                    CommentText = comment.CommentText,
                    UserName = user.FullName ?? "Anonymous User",
                    CreatedAt = comment.CreatedAt,
                    Reply = null
                };

                return Json(new { success = true, comment = commentDisplay, message = "Comment added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to add comment" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddReply([FromBody] AddReplyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid reply data" });
            }

            try
            {
                var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(sellerId))
                {
                    return Json(new { success = false, message = "Seller not authenticated" });
                }

                var seller = await _userManager.FindByIdAsync(sellerId);
                if (seller == null)
                {
                    return Json(new { success = false, message = "Seller not found" });
                }

                var comment = await _context.ProductComments
                    .Include(c => c.Reply)
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.CommentId == model.CommentId);

                if (comment == null)
                {
                    return Json(new { success = false, message = "Comment not found" });
                }

                if (comment.Product.SellerId != sellerId)
                {
                    return Json(new { success = false, message = "You can only reply to comments on your own products" });
                }

                if (comment.Reply != null)
                {
                    return Json(new { success = false, message = "This comment already has a reply" });
                }

                var reply = new ProductCommentReply
                {
                    CommentId = model.CommentId,
                    SellerId = sellerId,
                    ReplyText = model.ReplyText,
                    CreatedAt = DateTime.Now
                };

                _context.ProductCommentReplies.Add(reply);
                await _context.SaveChangesAsync();

                var replyDisplay = new ReplyDisplayViewModel
                {
                    ReplyId = reply.ReplyId,
                    ReplyText = reply.ReplyText,
                    SellerName = seller.BusinessName ?? seller.FullName ?? "Seller",
                    CreatedAt = reply.CreatedAt
                };

                return Json(new { success = true, reply = replyDisplay, message = "Reply added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to add reply" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> CanUserReply()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { canReply = false, reason = "Not authenticated" });
            }

            var canReply = User.IsInRole("Seller");
            return Json(new { canReply = canReply });
        }


        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        // GET: Display Cart Page
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItems = await _context.Carts
                    .Where(c => c.BuyerId == userId)
                    .Include(c => c.Product)
                        .ThenInclude(p => p.Seller)
                    .Select(c => new CartItemViewModel
                    {
                        ProductID = c.ProductId,
                        ProductName = c.Product.ProductName,
                        ProductPrice = c.Product.ProductPrice,
                        Quantity = c.Quantity,
                        Image = c.Product.Image,
                        SellerId = c.Product.SellerId,
                        SellerName = c.Product.Seller.FullName ?? "",
                        SellerBusinessName = c.Product.Seller.BusinessName ?? "",
                        AvailableStock = c.Product.ProductQuantity
                    })
                    .ToListAsync();

                var cartViewModel = new CartViewModel
                {
                    CartItems = cartItems
                };

                return View(cartViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load cart";
                return View(new CartViewModel());
            }
        }

        // POST: Add item to cart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = GetCurrentUserId();

                var product = await _context.Product.FindAsync(productId);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found";
                    return RedirectToAction("ProductDetails", new { id = productId });
                }

                if (product.ProductQuantity < quantity)
                {
                    TempData["ErrorMessage"] = $"Only {product.ProductQuantity} items available in stock";
                    return RedirectToAction("ProductDetails", new { id = productId });
                }

                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.BuyerId == userId && c.ProductId == productId);

                if (existingCart != null)
                {
                    var newQuantity = existingCart.Quantity + quantity;

                    if (newQuantity > product.ProductQuantity)
                    {
                        TempData["ErrorMessage"] = $"Cannot add more. Only {product.ProductQuantity} items available";
                        return RedirectToAction("ProductDetails", new { id = productId });
                    }

                    existingCart.Quantity = newQuantity;
                    existingCart.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Add new cart item
                    var cartItem = new Cart
                    {
                        BuyerId = userId,
                        ProductId = productId,
                        Quantity = quantity,
                        AddedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product added to cart successfully!";
                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to add product to cart";
                return RedirectToAction("ProductDetails", new { id = productId });
            }
        }

        // POST: Update cart item quantity
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateCartQuantity(int productId, int quantity)
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItem = await _context.Carts
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.BuyerId == userId && c.ProductId == productId);

                if (cartItem == null)
                {
                    TempData["ErrorMessage"] = "Cart item not found";
                    return RedirectToAction("Cart");
                }

                if (quantity <= 0)
                {

                    _context.Carts.Remove(cartItem);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Item removed from cart";
                    return RedirectToAction("Cart");
                }

                if (quantity > cartItem.Product.ProductQuantity)
                {
                    TempData["ErrorMessage"] = $"Only {cartItem.Product.ProductQuantity} items available";
                    return RedirectToAction("Cart");
                }

                cartItem.Quantity = quantity;
                cartItem.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cart updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update cart";
            }

            return RedirectToAction("Cart");
        }

        // POST: Remove item from cart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.BuyerId == userId && c.ProductId == productId);

                if (cartItem != null)
                {
                    _context.Carts.Remove(cartItem);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Item removed from cart!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to remove item from cart";
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItems = await _context.Carts
                    .Where(c => c.BuyerId == userId)
                    .ToListAsync();

                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cart cleared successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to clear cart";
            }

            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { count = 0 });
                }

                var userId = GetCurrentUserId();
                var count = await _context.Carts
                    .Where(c => c.BuyerId == userId)
                    .SumAsync(c => c.Quantity);

                return Json(new { count = count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }



        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get cart items
                var cartItems = await _context.Carts
                    .Where(c => c.BuyerId == userId)
                    .Include(c => c.Product)
                        .ThenInclude(p => p.Seller)
                    .Select(c => new CartItemViewModel
                    {
                        ProductID = c.ProductId,
                        ProductName = c.Product.ProductName,
                        ProductPrice = c.Product.ProductPrice,
                        Quantity = c.Quantity,
                        Image = c.Product.Image,
                        SellerId = c.Product.SellerId,
                        SellerName = c.Product.Seller.FullName ?? "",
                        SellerBusinessName = c.Product.Seller.BusinessName ?? "",
                        AvailableStock = c.Product.ProductQuantity
                    })
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty";
                    return RedirectToAction("Cart");
                }

                // Get user details for pre-filling form
                var user = await _userManager.FindByIdAsync(userId);

                var checkoutModel = new CheckoutViewModel
                {
                    CartItems = cartItems,
                    SubTotal = cartItems.Sum(i => i.SubTotal),
                    DeliveryFee = 60,
                    TotalAmount = cartItems.Sum(i => i.SubTotal) + 60,
                    TotalItems = cartItems.Sum(i => i.Quantity),
                    DeliveryName = user?.FullName ?? "",
                    DeliveryPhone = user?.PhoneNumber ?? "",
                    DeliveryAddress = user?.Location ?? "",
                    PaymentMethod = "COD"
                };

                return View(checkoutModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load checkout page";
                return RedirectToAction("Cart");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                model.CartItems = await _context.Carts
                    .Where(c => c.BuyerId == userId)
                    .Include(c => c.Product)
                    .Select(c => new CartItemViewModel
                    {
                        ProductID = c.ProductId,
                        ProductName = c.Product.ProductName,
                        ProductPrice = c.Product.ProductPrice,
                        Quantity = c.Quantity,
                        Image = c.Product.Image,
                        AvailableStock = c.Product.ProductQuantity
                    })
                    .ToListAsync();

                model.SubTotal = model.CartItems.Sum(i => i.SubTotal);
                model.TotalAmount = model.SubTotal + model.DeliveryFee;

                return View("Checkout", model);
            }

            try
            {
                var userId = GetCurrentUserId();

                var cartItems = await _context.Carts
                    .Where(c => c.BuyerId == userId)
                    .Include(c => c.Product)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty";
                    return RedirectToAction("Cart");
                }

                decimal subtotal = cartItems.Sum(c => c.Product.ProductPrice * c.Quantity);

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Product.ProductQuantity < cartItem.Quantity)
                    {
                        TempData["ErrorMessage"] = $"Insufficient stock for {cartItem.Product.ProductName}";
                        return RedirectToAction("Checkout");
                    }
                }

                var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                var order = new Order
                {
                    OrderNumber = orderNumber,
                    BuyerId = userId,
                    TotalAmount = subtotal,
                    DeliveryFee = model.DeliveryFee,
                    PaymentMethod = model.PaymentMethod,
                    OrderStatus = "Pending",
                    PaymentStatus = model.PaymentMethod == "COD" ? "Pending" : "Pending",
                    DeliveryName = model.DeliveryName,
                    DeliveryPhone = model.DeliveryPhone,
                    DeliveryAddress = model.DeliveryAddress,
                    DeliveryCity = model.DeliveryCity,
                    DeliveryState = model.DeliveryState,
                    DeliveryNote = model.DeliveryNote,
                    OrderDate = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order items
                var orderItemsList = new List<OrderItem>();
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.Product.ProductName,
                        Price = cartItem.Product.ProductPrice,
                        Quantity = cartItem.Quantity,
                        SellerId = cartItem.Product.SellerId,
                        ProductImage = cartItem.Product.Image
                    };

                    _context.OrderItems.Add(orderItem);
                    orderItemsList.Add(orderItem);
                    cartItem.Product.ProductQuantity -= cartItem.Quantity;
                }

                await _context.SaveChangesAsync(); // Save to get OrderItem IDs

                // ✅ CREATE COMMISSION RECORDS
                foreach (var orderItem in orderItemsList)
                {
                    decimal productAmount = orderItem.Price * orderItem.Quantity;
                    decimal commissionRate = 5.0m;
                    decimal commissionAmount = (productAmount * commissionRate) / 100;
                    decimal sellerEarning = productAmount - commissionAmount;

                    var commission = new AdminCommission
                    {
                        OrderId = order.OrderId,
                        OrderItemId = orderItem.OrderItemId,
                        SellerId = orderItem.SellerId,
                        BuyerId = userId,
                        ProductAmount = productAmount,
                        CommissionRate = commissionRate,
                        CommissionAmount = commissionAmount,
                        SellerEarning = sellerEarning,
                        OrderStatus = "Pending",
                        CreatedAt = DateTime.Now
                    };

                    _context.AdminCommissions.Add(commission);
                }

                // Create notifications for sellers
                var sellerGroups = cartItems.GroupBy(c => c.Product.SellerId);
                foreach (var sellerGroup in sellerGroups)
                {
                    var sellerId = sellerGroup.Key;
                    var sellerItems = sellerGroup.ToList();
                    var itemCount = sellerItems.Sum(c => c.Quantity);
                    var sellerTotal = sellerItems.Sum(c => c.Product.ProductPrice * c.Quantity);

                    var notification = new SellerNotification
                    {
                        SellerId = sellerId,
                        NotificationType = "NewOrder",
                        Title = "New Order Received",
                        Message = $"You have received a new order ({order.OrderNumber}) with {itemCount} item(s) worth Rs. {sellerTotal:N2}",
                        OrderId = order.OrderId,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    _context.SellerNotifications.Add(notification);
                }

                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("OrderConfirmation", new { orderNumber = orderNumber });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to place order. Please try again.";
                return RedirectToAction("Checkout");
            }
        }

        // GET: Order Confirmation Page
        // GET: Order Confirmation Page
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {
            try
            {
                var userId = GetCurrentUserId();

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && o.BuyerId == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found";
                    return RedirectToAction("Index");
                }

                // DEBUG: Log the order details
                Console.WriteLine($"OrderConfirmation - OrderId: {order.OrderId}, OrderNumber: {order.OrderNumber}");

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load order details";
                return RedirectToAction("Index");
            }
        }


        // GET: My Orders (Buyer View)
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            try
            {
                var userId = GetCurrentUserId();

                var orders = await _context.Orders
                    .Where(o => o.BuyerId == userId)
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load orders";
                return View(new List<Order>());
            }
        }

        // GET: Order Details
        [Authorize]
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.BuyerId == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found";
                    return RedirectToAction("MyOrders");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load order details";
                return RedirectToAction("MyOrders");
            }
        }

        // POST: Cancel Order
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.BuyerId == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found";
                    return RedirectToAction("MyOrders");
                }

                // Only allow cancellation if order is still pending
                if (order.OrderStatus != "Pending")
                {
                    TempData["ErrorMessage"] = "Cannot cancel order. Order already processed.";
                    return RedirectToAction("OrderDetails", new { id = orderId });
                }

                // Restore product stock
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.ProductQuantity += item.Quantity;
                    }
                }

                order.OrderStatus = "Cancelled";

                var orderItems = order.OrderItems.ToList();
                var sellerGroups = orderItems.GroupBy(oi => oi.SellerId);

                foreach (var sellerGroup in sellerGroups)
                {
                    var sellerId = sellerGroup.Key;

                    var notification = new SellerNotification
                    {
                        SellerId = sellerId,
                        NotificationType = "OrderCancelled",
                        Title = "Order Cancelled",
                        Message = $"Order {order.OrderNumber} has been cancelled by the buyer",
                        OrderId = order.OrderId,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    _context.SellerNotifications.Add(notification);
                }
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Order cancelled successfully";
                return RedirectToAction("MyOrders");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to cancel order";
                return RedirectToAction("MyOrders");
            }
        }

    }
}