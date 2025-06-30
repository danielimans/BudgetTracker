using System.Globalization;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string category, string month)
        {
            var expenses = _context.Expenses.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                expenses = expenses
                    .Where(e => EF.Functions.Like(e.Category, $"%{category}%"));
            }

            if (!string.IsNullOrEmpty(month))
            {
                if (DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime selectedMonth))
                {
                    expenses = expenses
                        .Where(e => e.Date.Year == selectedMonth.Year && e.Date.Month == selectedMonth.Month);
                }
            }

            return View(expenses.OrderByDescending(e => e.Date).ToList());

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Expense expense)
        {
            expense.Date = expense.Date == default ? DateTime.Now : expense.Date;
            _context.Expenses.Add(expense);
            _context.SaveChanges();

            TempData["Message"] = "Expense added successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        [HttpPost]
        public IActionResult Edit(Expense updatedExpense)
        {
            var expense = _context.Expenses.Find(updatedExpense.Id);
            if (expense == null) return NotFound();

            expense.Category = updatedExpense.Category;
            expense.Amount = updatedExpense.Amount;
            expense.Date = updatedExpense.Date;
            expense.Notes = updatedExpense.Notes;

            _context.SaveChanges();
            TempData["Message"] = "Expense updated!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                _context.SaveChanges();
            }

            TempData["Message"] = "Expense deleted.";
            return RedirectToAction("Index");
        }

        public static List<Expense> GetExpensesStatic()
        {
            return new List<Expense>(); 
        }
    }
}

