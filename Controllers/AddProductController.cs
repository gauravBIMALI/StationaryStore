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
                    ProductPrice = p.ProductPrice,           // Fixed: was Price
                    ProductDescription = p.ProductDescription, // Fixed: was Description
                    ProductQuantity = p.ProductQuantity,     // Added missing field
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

        // POST: AddProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // ADD THIS LINE - Get current seller's ID
                product.SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Set timestamps
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;

                // Handle image (your existing code)
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(ms);
                        byte[] imageBytes = ms.ToArray();
                        product.Image = Convert.ToBase64String(imageBytes);
                    }
                }

                _context.Product.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType");
            return View(product);
        }

        // GET: AddProduct/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // GET: AddProduct/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Product.FindAsync(id);
            if (product == null) return NotFound();

            //seller can only edit their own products
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.SellerId != userId)
            {
                return Forbid(); // or return NotFound()
            }

            ViewBag.Categories = new SelectList(_context.Category, "CategoryType", "CategoryType", product.CategoryType);
            return View(product);
        }

        // POST: AddProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.ProductID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Product.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == id);

                    if (existingProduct == null) return NotFound();

                    //seller can only edit their own products
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (existingProduct.SellerId != userId)
                    {
                        return Forbid();
                    }

                    //Preserve the SellerId
                    product.SellerId = existingProduct.SellerId;

                    // Update timestamps
                    product.CreatedAt = existingProduct.CreatedAt; // Preserve original creation date
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

            var product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: AddProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}