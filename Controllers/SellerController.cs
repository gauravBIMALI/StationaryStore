using ClzProject.Models;
using ClzProject.ViewModels;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserRoles.Data;
using UserRoles.Models;


namespace ClzProject.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SellerController(AppDbContext context, SignInManager<Users> signInManager, UserManager<Users> userManager, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        private string GetSellerId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }


        //This is for edit profile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new SellerProfileViewModel
            {
                Name = user.FullName,
                Email = user.Email ?? string.Empty,
                //Age = (int)user.Age,
                Age = user.Age ?? 0,
                Location = user.Location ?? string.Empty,
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessType = user.BusinessType ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                ProfileImageBase64 = user.ProfileImage

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(SellerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Handle image upload
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                // Validate image
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(model.ProfileImage.ContentType))
                {
                    ModelState.AddModelError("ProfileImage", "Only JPG, PNG or GIF images are allowed.");
                    return View(model);
                }

                // Limit file size to 2MB
                if (model.ProfileImage.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfileImage", "Image must be smaller than 2MB.");
                    return View(model);
                }

                // Convert to Base64
                using (var memoryStream = new MemoryStream())
                {
                    await model.ProfileImage.CopyToAsync(memoryStream);
                    user.ProfileImage = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            // Update other profile fields
            user.FullName = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Age = model.Age;
            user.Location = model.Location;
            user.BusinessName = model.BusinessName;
            user.BusinessType = model.BusinessType;
            user.PhoneNumber = model.Phone;

            // Save changes
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        public IActionResult SellerDltProfile()
        {
            return View();
        }

        // GET: Display Profile
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new SellerProfileViewModel
            {
                Name = user.FullName,
                Email = user.Email,
                //Age = (int)user.Age,
                Age = user.Age ?? 0,
                Location = user.Location ?? string.Empty,
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessType = user.BusinessType ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                ProfileImageBase64 = user.ProfileImage
                // Base64 string from database
            };

            return View(model);
        }

        // Delete Profile Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DltProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await signInManager.SignOutAsync();

                return RedirectToAction("Login", "Account");
            }


            return RedirectToAction("EditProfile");
        }




        //[HttpPost]
        //[Authorize(Roles = "Seller")]
        //public async Task<IActionResult> MarkAsRead(int id)
        //{
        //    var notification = await _context.ProductDeletionNotifications.FindAsync(id);
        //    if (notification != null && notification.SellerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
        //    {
        //        notification.IsRead = true;
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction(nameof(Notifications));
        //}

        public async Task<IActionResult> Dashboard()
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get seller's products count
            var productsCount = await _context.Product
                .Where(p => p.SellerId == sellerId)
                .CountAsync();

            // Get pending comments count (comments without replies on seller's products)
            var pendingCommentsCount = await _context.ProductComments
                .Where(c => c.Product.SellerId == sellerId && c.Reply == null)
                .CountAsync();

            // Get total comments on seller's products
            var totalCommentsCount = await _context.ProductComments
                .Where(c => c.Product.SellerId == sellerId)
                .CountAsync();

            // Get recent comments
            var recentComments = await _context.ProductComments
                .Where(c => c.Product.SellerId == sellerId)
                .Include(c => c.User)
                .Include(c => c.Product)
                .Include(c => c.Reply)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .Select(c => new SellerCommentViewModel
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    UserName = c.User.FullName ?? "Anonymous User",
                    ProductId = c.ProductId,
                    ProductName = c.Product.ProductName,
                    CreatedAt = c.CreatedAt,
                    HasReply = c.Reply != null
                })
                .ToListAsync();

            // NEW: Get Orders Stats
            var orderItemsQuery = _context.OrderItems
                .Where(oi => oi.SellerId == sellerId)
                .Include(oi => oi.Order);

            var totalOrders = await orderItemsQuery
                .Select(oi => oi.OrderId)
                .Distinct()
                .CountAsync();

            var pendingOrders = await orderItemsQuery
                .Where(oi => oi.Order.OrderStatus == "Pending")
                .Select(oi => oi.OrderId)
                .Distinct()
                .CountAsync();

            var shippedOrders = await orderItemsQuery
                .Where(oi => oi.Order.OrderStatus == "Shipped")
                .Select(oi => oi.OrderId)
                .Distinct()
                .CountAsync();

            var totalRevenue = await orderItemsQuery
                .Where(oi => oi.Order.OrderStatus == "Delivered")
                .SumAsync(oi => oi.Price * oi.Quantity);

            // NEW: Get Recent Orders
            var recentOrders = await _context.OrderItems
                .Where(oi => oi.SellerId == sellerId)
                .Include(oi => oi.Order)
                .ThenInclude(o => o.Buyer)
                .Include(oi => oi.Product)
                .OrderByDescending(oi => oi.Order.OrderDate)
                .Take(5)
                .Select(oi => new SellerOrderViewModel
                {
                    OrderId = oi.OrderId,
                    OrderItemId = oi.OrderItemId,
                    OrderNumber = oi.Order.OrderNumber,
                    ProductName = oi.ProductName,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    Total = oi.Price * oi.Quantity,
                    OrderDate = oi.Order.OrderDate,
                    OrderStatus = oi.Order.OrderStatus,
                    PaymentMethod = oi.Order.PaymentMethod,
                    BuyerName = oi.Order.Buyer.FullName ?? "",
                    DeliveryName = oi.Order.DeliveryName,
                    DeliveryPhone = oi.Order.DeliveryPhone,
                    DeliveryAddress = oi.Order.DeliveryAddress,
                    DeliveryCity = oi.Order.DeliveryCity,
                    ProductImage = oi.ProductImage
                })
                .ToListAsync();

            // NEW: Get Notifications
            var unreadNotifications = await _context.SellerNotifications
                .Where(n => n.SellerId == sellerId && !n.IsRead)
                .CountAsync();

            var recentNotifications = await _context.SellerNotifications
                .Where(n => n.SellerId == sellerId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            var dashboardData = new SellerDashboardViewModel
            {
                TotalProducts = productsCount,
                PendingComments = pendingCommentsCount,
                TotalComments = totalCommentsCount,
                RecentComments = recentComments,

                // NEW: Orders Data
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                ShippedOrders = shippedOrders,
                TotalRevenue = totalRevenue,
                RecentOrders = recentOrders,

                // NEW: Notifications
                UnreadNotifications = unreadNotifications,
                RecentNotifications = recentNotifications
            };

            return View(dashboardData);
        }

        // View Comments on Seller's Products - CORRECTED VERSION
        public async Task<IActionResult> Comments(int page = 1, int pageSize = 10, string filter = "all")
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Start with base query
            IQueryable<ProductComment> commentsQuery = _context.ProductComments
                .Where(c => c.Product.SellerId == sellerId)
                .Include(c => c.User)
                .Include(c => c.Product)
                .Include(c => c.Reply);

            // Apply filter without casting issues
            if (filter.ToLower() == "pending")
            {
                commentsQuery = commentsQuery.Where(c => c.Reply == null);
            }
            else if (filter.ToLower() == "replied")
            {
                commentsQuery = commentsQuery.Where(c => c.Reply != null);
            }
            // "all" - no additional filter needed

            // Order the results
            commentsQuery = commentsQuery.OrderByDescending(c => c.CreatedAt);

            var totalComments = await commentsQuery.CountAsync();

            var comments = await commentsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new SellerCommentViewModel
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    UserName = c.User.FullName ?? "Anonymous User",
                    ProductId = c.ProductId,
                    ProductName = c.Product.ProductName,
                    CreatedAt = c.CreatedAt,
                    HasReply = c.Reply != null,
                    ReplyText = c.Reply != null ? c.Reply.ReplyText : null,
                    ReplyDate = c.Reply != null ? c.Reply.CreatedAt : null
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalComments / pageSize);
            ViewBag.TotalComments = totalComments;
            ViewBag.CurrentFilter = filter;

            return View(comments);
        }

        // Get comments for specific product (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetProductComments(int productId)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verify the product belongs to the current seller
            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.ProductID == productId && p.SellerId == sellerId);

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found or access denied" });
            }

            var comments = await _context.ProductComments
                .Where(c => c.ProductId == productId)
                .Include(c => c.User)
                .Include(c => c.Reply)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    UserName = c.User.FullName ?? "Anonymous User",
                    CreatedAt = c.CreatedAt,
                    HasReply = c.Reply != null,
                    ReplyText = c.Reply != null ? c.Reply.ReplyText : null,
                    //ReplyDate = c.Reply != null ? c.Reply.CreatedAt : null
                })
                .ToListAsync();

            return Json(new { success = true, comments = comments, productName = product.ProductName });
        }

        [HttpPost]
        public async Task<IActionResult> ReplyToComment([FromBody] SellerReplyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid reply data" });
            }

            try
            {
                var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var seller = await userManager.FindByIdAsync(sellerId);

                // Verify the comment belongs to seller's product
                var comment = await _context.ProductComments
                    .Include(c => c.Product)
                    .Include(c => c.Reply)
                    .FirstOrDefaultAsync(c => c.CommentId == model.CommentId);

                if (comment == null)
                {
                    return Json(new { success = false, message = "Comment not found" });
                }

                // IMPORTANT: Only the product owner can reply
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

                return Json(new
                {
                    success = true,
                    message = "Reply added successfully",
                    reply = new
                    {
                        ReplyText = reply.ReplyText,
                        CreatedAt = reply.CreatedAt,
                        SellerName = seller.BusinessName ?? seller.FullName ?? "Seller"
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to add reply" });
            }
        }

        // View Seller's Products with Comment Statistics
        public async Task<IActionResult> Products()
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var products = await _context.Product
                .Where(p => p.SellerId == sellerId)
                .Select(p => new SellerProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductQuantity = p.ProductQuantity,
                    CategoryType = p.CategoryType,
                    CreatedAt = p.CreatedAt,
                    CommentsCount = _context.ProductComments.Count(c => c.ProductId == p.ProductID),
                    UnrepliedCommentsCount = _context.ProductComments.Count(c => c.ProductId == p.ProductID && c.Reply == null)
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }

        // GET: Seller Orders
        public async Task<IActionResult> Orders(string status = "All")
        {
            try
            {
                var sellerId = GetSellerId();

                var ordersQuery = _context.OrderItems
                    .Where(oi => oi.SellerId == sellerId)
                    .Include(oi => oi.Order)
                    .ThenInclude(o => o.Buyer)
                    .Include(oi => oi.Product)
                    .AsQueryable();

                if (status != "All")
                {
                    ordersQuery = ordersQuery.Where(oi => oi.Order.OrderStatus == status);
                }

                var orders = await ordersQuery
                    .OrderByDescending(oi => oi.Order.OrderDate)
                    .Select(oi => new SellerOrderViewModel
                    {
                        OrderId = oi.OrderId,
                        OrderItemId = oi.OrderItemId,
                        OrderNumber = oi.Order.OrderNumber,
                        ProductName = oi.ProductName,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        Total = oi.Price * oi.Quantity,
                        OrderDate = oi.Order.OrderDate,
                        OrderStatus = oi.Order.OrderStatus,
                        PaymentMethod = oi.Order.PaymentMethod,
                        BuyerName = oi.Order.Buyer.FullName ?? "",
                        BuyerPhone = oi.Order.Buyer.PhoneNumber ?? "",
                        DeliveryName = oi.Order.DeliveryName,
                        DeliveryPhone = oi.Order.DeliveryPhone,
                        DeliveryAddress = oi.Order.DeliveryAddress,
                        DeliveryCity = oi.Order.DeliveryCity,
                        DeliveryState = oi.Order.DeliveryState,
                        DeliveryNote = oi.Order.DeliveryNote,
                        ProductImage = oi.ProductImage
                    })
                    .ToListAsync();

                ViewBag.CurrentStatus = status;
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load orders";
                return View(new List<SellerOrderViewModel>());
            }
        }

        // GET: Order Details
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var sellerId = GetSellerId();

                var orderItem = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .ThenInclude(o => o.Buyer)
                    .Include(oi => oi.Product)
                    .FirstOrDefaultAsync(oi => oi.OrderItemId == id && oi.SellerId == sellerId);

                if (orderItem == null)
                {
                    TempData["ErrorMessage"] = "Order not found";
                    return RedirectToAction("Orders");
                }

                var viewModel = new SellerOrderViewModel
                {
                    OrderId = orderItem.OrderId,
                    OrderItemId = orderItem.OrderItemId,
                    OrderNumber = orderItem.Order.OrderNumber,
                    ProductName = orderItem.ProductName,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    Price = orderItem.Price,
                    Total = orderItem.Price * orderItem.Quantity,
                    OrderDate = orderItem.Order.OrderDate,
                    OrderStatus = orderItem.Order.OrderStatus,
                    PaymentMethod = orderItem.Order.PaymentMethod,
                    BuyerName = orderItem.Order.Buyer.FullName ?? "",
                    BuyerPhone = orderItem.Order.Buyer.PhoneNumber ?? "",
                    DeliveryName = orderItem.Order.DeliveryName,
                    DeliveryPhone = orderItem.Order.DeliveryPhone,
                    DeliveryAddress = orderItem.Order.DeliveryAddress,
                    DeliveryCity = orderItem.Order.DeliveryCity,
                    DeliveryState = orderItem.Order.DeliveryState,
                    DeliveryNote = orderItem.Order.DeliveryNote,
                    ProductImage = orderItem.ProductImage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load order details";
                return RedirectToAction("Orders");
            }
        }

        // GET: Notifications
        public async Task<IActionResult> Notifications()
        {
            try
            {
                var sellerId = GetSellerId();

                var notifications = await _context.SellerNotifications
                    .Where(n => n.SellerId == sellerId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return View(notifications);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load notifications";
                return View(new List<SellerNotification>());
            }
        }

        // POST: Mark Notification as Read
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var sellerId = GetSellerId();

                var notification = await _context.SellerNotifications
                    .FirstOrDefaultAsync(n => n.NotificationId == id && n.SellerId == sellerId);

                if (notification != null)
                {
                    notification.IsRead = true;
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        // POST: Mark All Notifications as Read
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var sellerId = GetSellerId();

                var notifications = await _context.SellerNotifications
                    .Where(n => n.SellerId == sellerId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "All notifications marked as read";
                return RedirectToAction("Notifications");
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to mark notifications as read";
                return RedirectToAction("Notifications");
            }
        }

        // GET: Get Unread Notification Count
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var sellerId = GetSellerId();

                var count = await _context.SellerNotifications
                    .Where(n => n.SellerId == sellerId && !n.IsRead)
                    .CountAsync();

                return Json(new { count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }


    }
}
