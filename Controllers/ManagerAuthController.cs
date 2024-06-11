using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using terrain.Models;

namespace terrain.Controllers
{
    public class ManagerAuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View for Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handling login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ManagerLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var manager = await _context.Managers.SingleOrDefaultAsync(m => m.Nom == model.Name);
                if (manager != null && BCrypt.Net.BCrypt.Verify(model.Password, manager.Password))
                {
                    // Simulating token generation for demonstration purposes
                    TempData["Message"] = "Login successful!";
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }
    }

    public class ManagerLoginModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
