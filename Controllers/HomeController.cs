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


        public async Task<IActionResult> Index()
        {
            var products = await _context.Product
                .Where(p => p.ProductQuantity > 0)
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
                    // Image is intentionally excluded to speed up initial load
                })
                .ToListAsync();

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
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}