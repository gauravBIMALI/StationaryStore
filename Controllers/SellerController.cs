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


        //notification
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Notifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notifications = await _context.ProductDeletionNotifications
                .Where(n => n.SellerId == userId)
                .OrderByDescending(n => n.DeletedAt)
                .ToListAsync();

            return View(notifications);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.ProductDeletionNotifications.FindAsync(id);
            if (notification != null && notification.SellerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Notifications));
        }

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

            var dashboardData = new SellerDashboardViewModel
            {
                TotalProducts = productsCount,
                PendingComments = pendingCommentsCount,
                TotalComments = totalCommentsCount,
                RecentComments = recentComments
            };

            return View(dashboardData);
        }

        // View Comments on Seller's Products
        public async Task<IActionResult> Comments(int page = 1, int pageSize = 10, string filter = "all")
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var commentsQuery = _context.ProductComments
                .Where(c => c.Product.SellerId == sellerId)
                .Include(c => c.User)
                .Include(c => c.Product)
                .Include(c => c.Reply);

            // Apply filter
            switch (filter.ToLower())
            {
                case "pending":
                    commentsQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductComment, ProductCommentReply?>)commentsQuery.Where(c => c.Reply == null);
                    break;
                case "replied":
                    commentsQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductComment, ProductCommentReply?>)commentsQuery.Where(c => c.Reply != null);
                    break;
                    // "all" shows everything - no additional filter needed
            }

            commentsQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductComment, ProductCommentReply?>)commentsQuery.OrderByDescending(c => c.CreatedAt);

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

        // Reply to comment (AJAX) - Only product owner can reply
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

    }
}
