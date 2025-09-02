using ClzProject.Models;
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

        // NEW: API endpoint to get image for specific product
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
                .Include(p => p.Seller) // Include seller information
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Home/Category (Filter by category - WITHOUT IMAGES)
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
                    // Image excluded for faster loading
                })
                .ToListAsync();

            ViewBag.CategoryName = categoryType;
            return View("Index", products);
        }

        // GET: Search products (WITHOUT IMAGES)
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
                    // Image excluded for faster loading
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

        // Update the GetRelatedProducts method in your HomeController
        [HttpGet]
        public async Task<IActionResult> GetRelatedProducts(string categoryType, int excludeId, int count = 6)
        {
            try
            {
                var relatedProducts = await _context.Product
                    .Where(p => p.CategoryType == categoryType &&
                               p.ProductID != excludeId &&
                               p.ProductQuantity > 0)
                    .OrderBy(x => Guid.NewGuid()) // Random order
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
        // GET: Load comments for a product
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
                // Fix: Use User.FindFirst(ClaimTypes.NameIdentifier)?.Value instead of _userManager.GetUserId(User)
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

                // Return the new comment data
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

        // POST: Add reply to comment (Only sellers can reply)
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

                // Check if comment exists and verify it belongs to seller's product
                var comment = await _context.ProductComments
                    .Include(c => c.Reply)
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.CommentId == model.CommentId);

                if (comment == null)
                {
                    return Json(new { success = false, message = "Comment not found" });
                }

                // CRITICAL CHECK: Only the product owner can reply
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

                // Return the new reply data
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

        // GET: Check if current user can reply to comments (is seller)
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
    }
}