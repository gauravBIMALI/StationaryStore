// Alternative BuyerContactController with different method name
using ClzProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;
using UserRoles.Models;

namespace ClzProject.Controllers
{
    //[Authorize(Roles = "Buyer")]
    public class BuyerContactController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly IWebHostEnvironment _environment;

        public BuyerContactController(AppDbContext context, UserManager<Users> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: BuyerContact/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BuyerContact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuyerContactMessage model, IFormFile citizenshipDocument, IFormFile panDocument)
        {
            // Remove model validation for file paths since they will be set after upload
            ModelState.Remove("CitizenshipDocumentPath");
            ModelState.Remove("PANDocumentPath");

            // Validate required files
            if (citizenshipDocument == null || citizenshipDocument.Length == 0)
            {
                ModelState.AddModelError("CitizenshipDocument", "Citizenship document is required.");
            }
            else if (!IsValidFileType(citizenshipDocument) || !IsValidFileSize(citizenshipDocument))
            {
                ModelState.AddModelError("CitizenshipDocument", "Invalid file type or size. Please upload JPG, PNG, GIF, PDF, DOC, or DOCX files under 5MB.");
            }

            if (panDocument == null || panDocument.Length == 0)
            {
                ModelState.AddModelError("PANDocument", "PAN document is required.");
            }
            else if (!IsValidFileType(panDocument) || !IsValidFileSize(panDocument))
            {
                ModelState.AddModelError("PANDocument", "Invalid file type or size. Please upload JPG, PNG, GIF, PDF, DOC, or DOCX files under 5MB.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create Images directory if it doesn't exist
                    var imagesPath = Path.Combine(_environment.WebRootPath, "Images");
                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }

                    // Save Citizenship Document
                    var citizenshipFileName = $"citizenship_{Guid.NewGuid()}_{GetSafeFileName(citizenshipDocument.FileName)}";
                    var citizenshipFilePath = Path.Combine(imagesPath, citizenshipFileName);
                    using (var stream = new FileStream(citizenshipFilePath, FileMode.Create))
                    {
                        await citizenshipDocument.CopyToAsync(stream);
                    }
                    model.CitizenshipDocumentPath = $"/Images/{citizenshipFileName}";

                    // Save PAN Document
                    var panFileName = $"pan_{Guid.NewGuid()}_{GetSafeFileName(panDocument.FileName)}";
                    var panFilePath = Path.Combine(imagesPath, panFileName);
                    using (var stream = new FileStream(panFilePath, FileMode.Create))
                    {
                        await panDocument.CopyToAsync(stream);
                    }
                    model.PANDocumentPath = $"/Images/{panFileName}";

                    model.SubmittedAt = DateTime.Now;

                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Your message has been sent to admin successfully!";
                    return RedirectToAction(nameof(MySubmissions));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving your message. Please try again.");
                    // Log the exception here if you have logging configured
                    // You might want to log: ex.Message or ex.ToString()
                }
            }

            return View(model);
        }

        // GET: BuyerContact/MySubmissions
        public async Task<IActionResult> MySubmissions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var submissions = await _context.BuyerContactMessages
                .Where(m => m.Email == currentUser.Email)
                .OrderByDescending(m => m.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        // GET: BuyerContact/ViewDetails/5 
        public async Task<IActionResult> ViewDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var message = await _context.BuyerContactMessages
                .FirstOrDefaultAsync(m => m.Id == id && m.Email == currentUser.Email);

            if (message == null)
            {
                return NotFound();
            }
            return View(message);

        }

        // Helper methods
        private bool IsValidFileType(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private bool IsValidFileSize(IFormFile file)
        {
            return file.Length <= 5 * 1024 * 1024; // 5MB
        }

        private string GetSafeFileName(string fileName)
        {
            return Path.GetFileName(fileName).Replace(" ", "_");
        }
    }
}