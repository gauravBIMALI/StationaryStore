using ClzProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UserRoles.Data;
using UserRoles.Models;
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
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductID == id);
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
        public IActionResult User()
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


    }
}