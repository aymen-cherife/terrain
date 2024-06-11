using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using terrain.Models;

namespace terrain.Controllers
{
    public class UserAuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View for Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handling registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
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
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
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

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;
    }

    public class UserLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
