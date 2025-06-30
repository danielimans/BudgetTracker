using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BudgetTracker.Data;
using ExpenseTracker.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                var result = _hasher.VerifyHashedPassword(user, user.Password, password);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("Username", username);
                    TempData["Message"] = "Login successful!";
                    return RedirectToAction("Dashboard", "Home");
                }
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists!";
                return View();
            }

            var newUser = new User
            {
                Username = username
            };

            newUser.Password = _hasher.HashPassword(newUser, password);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["Message"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "Logout successful!";
            return RedirectToAction("Index", "Home");
        }
    }
}


