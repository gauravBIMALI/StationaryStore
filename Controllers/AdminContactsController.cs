using ClzProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;

namespace ClzProject.Controllers
{

    public class AdminContactsController : Controller
    {
        private readonly AppDbContext _context;

        public AdminContactsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: AdminContacts
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdminContact.ToListAsync());
        }


        // GET: AdminContacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminContact = await _context.AdminContact
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (adminContact == null)
            {
                return NotFound();
            }

            return View(adminContact);
        }

        // GET: AdminContacts/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminContact adminContact)
        {
            try
            {
                // Debug: Log received data
                Console.WriteLine($"Received: Name={adminContact.Name}, Email={adminContact.Email}");
                Console.WriteLine($"PAN File: {adminContact.PANFile?.FileName ?? "null"}");
                Console.WriteLine($"Verified ID File: {adminContact.VerifiedIDFile?.FileName ?? "null"}");

                // Handle file uploads
                if (adminContact.PANFile != null && adminContact.PANFile.Length > 0)
                {
                    adminContact.PANBase64 = await ConvertToBase64(adminContact.PANFile);
                    Console.WriteLine($"PAN Base64 length: {adminContact.PANBase64.Length}");
                }
                else
                {
                    // Set empty string if no file uploaded
                    adminContact.PANBase64 = string.Empty;
                }

                if (adminContact.VerifiedIDFile != null && adminContact.VerifiedIDFile.Length > 0)
                {
                    adminContact.VerifiedIDBase64 = await ConvertToBase64(adminContact.VerifiedIDFile);
                    Console.WriteLine($"Verified ID Base64 length: {adminContact.VerifiedIDBase64.Length}");
                }
                else
                {
                    // Set empty string if no file uploaded
                    adminContact.VerifiedIDBase64 = string.Empty;
                }

                // Remove file validation errors since we're using Base64
                ModelState.Remove("PANFile");
                ModelState.Remove("VerifiedIDFile");

                // Debug: Check ModelState
                if (!ModelState.IsValid)
                {
                    foreach (var modelError in ModelState)
                    {
                        Console.WriteLine($"ModelState Error - Key: {modelError.Key}");
                        foreach (var error in modelError.Value.Errors)
                        {
                            Console.WriteLine($"Error: {error.ErrorMessage}");
                        }
                    }
                    return View(adminContact);
                }

                Console.WriteLine("ModelState is valid, saving to database...");
                _context.Add(adminContact);
                await _context.SaveChangesAsync();
                Console.WriteLine("Successfully saved to database");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Create: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while saving the data.");
                return View(adminContact);
            }
        }

        // GET: AdminContacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminContact = await _context.AdminContact.FindAsync(id);
            if (adminContact == null)
            {
                return NotFound();
            }
            return View(adminContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminContact adminContact)
        {
            if (id != adminContact.ContactId)
            {
                return NotFound();
            }

            // Handle file uploads for edit
            if (adminContact.PANFile != null)
            {
                adminContact.PANBase64 = await ConvertToBase64(adminContact.PANFile);
            }

            if (adminContact.VerifiedIDFile != null)
            {
                adminContact.VerifiedIDBase64 = await ConvertToBase64(adminContact.VerifiedIDFile);
            }

            // Remove file validation errors
            ModelState.Remove("PANFile");
            ModelState.Remove("VerifiedIDFile");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminContact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminContactExists(adminContact.ContactId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adminContact);
        }

        // GET: AdminContacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminContact = await _context.AdminContact
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (adminContact == null)
            {
                return NotFound();
            }

            return View(adminContact);
        }

        // POST: AdminContacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminContact = await _context.AdminContact.FindAsync(id);
            if (adminContact != null)
            {
                _context.AdminContact.Remove(adminContact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminContactExists(int id)
        {
            return _context.AdminContact.Any(e => e.ContactId == id);
        }

        // Helper method to convert file to Base64
        private async Task<string> ConvertToBase64(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            return Convert.ToBase64String(fileBytes);
        }
    }
}