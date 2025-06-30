using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var budgets = _context.Budgets.ToList();
            var expenses = _context.Expenses.ToList();

            var viewModel = budgets.Select(b =>
            {
                decimal totalSpent = 0;

                if (!DateTime.TryParse("01 " + b.Month, out DateTime monthStart))
                    monthStart = DateTime.Now;

                if (b.Type == "Monthly")
                {
                    totalSpent = expenses
                        .Where(e => e.Date.Month == monthStart.Month && e.Date.Year == monthStart.Year)
                        .Sum(e => e.Amount);
                }
                else if (b.Type == "Weekly")
                {
                    DateTime weekStart = monthStart;
                    DateTime weekEnd = weekStart.AddDays(6);

                    totalSpent = expenses
                        .Where(e => e.Date.Date >= weekStart.Date && e.Date.Date <= weekEnd.Date)
                        .Sum(e => e.Amount);
                }

                return new BudgetStatusViewModel
                {
                    Budget = b,
                    TotalSpent = totalSpent,
                    Remaining = b.Amount - totalSpent,
                    IsOverBudget = totalSpent > b.Amount
                };
            }).ToList();

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new Budget());
        }

        [HttpPost]
        public IActionResult Create(Budget budget)
        {
            if (!string.IsNullOrEmpty(budget.Month))
            {
                var parsed = DateTime.Parse(budget.Month + "-01");
                budget.Month = parsed.ToString("MMMM yyyy");
            }

            _context.Budgets.Add(budget);
            _context.SaveChanges();

            TempData["Message"] = "Budget added successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var budget = _context.Budgets.Find(id);
            if (budget == null) return NotFound();

            var parsed = DateTime.Parse("01 " + budget.Month);
            budget.Month = parsed.ToString("yyyy-MM");

            return View(budget);
        }

        [HttpPost]
        public IActionResult Edit(Budget updated)
        {
            if (!string.IsNullOrEmpty(updated.Month))
            {
                var parsed = DateTime.Parse(updated.Month + "-01");
                updated.Month = parsed.ToString("MMMM yyyy");
            }

            var budget = _context.Budgets.Find(updated.Id);
            if (budget == null) return NotFound();

            budget.Month = updated.Month;
            budget.Amount = updated.Amount;
            budget.Notes = updated.Notes;
            budget.Type = updated.Type;

            _context.SaveChanges();
            TempData["Message"] = "Budget updated!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var budget = _context.Budgets.Find(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                _context.SaveChanges();
            }

            TempData["Message"] = "Budget deleted.";
            return RedirectToAction("Index");
        }
    }

    public class BudgetStatusViewModel
    {
        public Budget Budget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal Remaining { get; set; }
        public bool IsOverBudget { get; set; }
    }
}
