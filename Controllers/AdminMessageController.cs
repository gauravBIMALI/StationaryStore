using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;
using UserRoles.Models;

namespace ClzProject.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminMessageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly IWebHostEnvironment _environment;

        public AdminMessageController(AppDbContext context, UserManager<Users> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: AdminMessage
        public async Task<IActionResult> Index()
        {
            var messages = await _context.BuyerContactMessages
                .Where(m => !m.IsReplied)
                .OrderByDescending(m => m.SubmittedAt)
                .ToListAsync();

            return View(messages);
        }

        // GET: AdminMessage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.BuyerContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            // Pass filenames to ViewBag
            ViewBag.CitizenshipFileName = Path.GetFileName(message.CitizenshipDocumentPath);
            ViewBag.PANFileName = Path.GetFileName(message.PANDocumentPath);

            return View(message);
        }

        // GET: AdminMessage/Reply/5
        public async Task<IActionResult> Reply(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.BuyerContactMessages
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsReplied);

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: AdminMessage/Reply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string adminReply)
        {
            var message = await _context.BuyerContactMessages
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsReplied);

            if (message == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(adminReply))
            {
                ModelState.AddModelError("", "Reply message cannot be empty.");
                return View(message);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            message.AdminReply = adminReply;
            message.IsReplied = true;
            message.RepliedAt = DateTime.Now;
            message.AdminUserId = currentUser?.Id;

            _context.Update(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reply sent successfully. Message has been removed from the pending list.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Display document
        public async Task<IActionResult> ViewDocument(int id, string docType)
        {
            var message = await _context.BuyerContactMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            string filePath = docType.ToLower() == "citizenship"
                ? message.CitizenshipDocumentPath
                : message.PANDocumentPath;

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var contentType = GetContentType(fullPath);
            var fileName = Path.GetFileName(fullPath);

            return File(fileBytes, contentType, fileName);
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream",
            };
        }
    }
}