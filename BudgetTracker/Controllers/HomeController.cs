using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Dashboard page
        public IActionResult Dashboard()
        {
            // Restrict access to logged-in users
            if (HttpContext.Session.GetString("Username") == null)
            {
                TempData["Message"] = "Please log in to access the dashboard.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}

