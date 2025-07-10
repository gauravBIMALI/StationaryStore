using Microsoft.AspNetCore.Mvc;

namespace ClzProject.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult ManageSeller()
        {
            return View();
        }
    }
}
