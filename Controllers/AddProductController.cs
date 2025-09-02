using ClzProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserRoles.Data;

namespace ClzProject.Controllers
{
    [Authorize(Roles = "Seller")]
    public class AddProductController : Controller
    {
        private readonly AppDbContext _context;

        public AddProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Load products WITHOUT the Image field to make it fast
            var products = await _context.Product
                .Where(p => p.SellerId == userId)
                .Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductDescription = p.ProductDescription,
                    ProductQuantity = p.ProductQuantity,
                    CategoryType = p.CategoryType,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    SellerId = p.SellerId
                    // Deliberately NOT selecting Image field
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }

        // Add this new method for loading images separately
        [HttpGet]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var image = await _context.Product
                .Where(p => p.ProductID == id && p.SellerId == userId)
                .Select(p => p.Image)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(image))
                return NotFound();

            return Json(new { imageBase64 = image });
        }

        // GET: AddProduct/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType");
            return View();
        }

        // POST: AddProduct/Create - FIXED VERSION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            // Remove SellerId from ModelState validation since we set it manually
            ModelState.Remove("SellerId");
            ModelState.Remove("Seller");

            product.SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            // Debug: Check what's in ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Field: {error.Field}, Errors: {string.Join(", ", error.Errors)}");
                }
            }

            if (ModelState.IsValid)
            {
                // Set timestamps
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;

                // Handle image
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("imageFile", "Please upload a valid image file (JPG, JPEG, PNG, GIF)");
                        ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType");
                        return View(product);
                    }

                    // Validate file size (e.g., max 5MB)
                    if (imageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("imageFile", "File size must be less than 5MB");
                        ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType");
                        return View(product);
                    }

                    using (var ms = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(ms);
                        byte[] imageBytes = ms.ToArray();
                        product.Image = Convert.ToBase64String(imageBytes);
                    }
                }

                try
                {
                    _context.Product.Add(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product created successfully!";
                    return RedirectToAction(nameof(Create));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"Error creating product: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the product. Please try again.");
                }
            }
            ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType", product.CategoryType);
            return View(product);
        }

        // GET: AddProduct/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id && m.SellerId == userId);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: AddProduct/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.ProductID == id && p.SellerId == userId);

            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType", product.CategoryType);
            return View(product);
        }

        // POST: AddProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.ProductID) return NotFound();

            // Remove navigation properties from ModelState validation
            ModelState.Remove("Seller");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Product
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.ProductID == id && p.SellerId == userId);

                    if (existingProduct == null) return NotFound();

                    // Preserve the SellerId and CreatedAt
                    product.SellerId = existingProduct.SellerId;
                    product.CreatedAt = existingProduct.CreatedAt;
                    product.UpdatedAt = DateTime.Now;

                    // Handle image update
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(ms);
                            product.Image = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    else
                    {
                        product.Image = existingProduct.Image;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Product.Any(e => e.ProductID == id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType", product.CategoryType);
            return View(product);
        }

        // GET: AddProduct/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id && m.SellerId == userId);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: AddProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.ProductID == id && p.SellerId == userId);

            if (product != null)
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}