using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Helpers;
using QuestPDF.Fluent;
using BudgetTracker.Data;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTracker.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string month)
        {
            var model = GetReportViewModel(month);
            return View(model);
        }

        public IActionResult ExportToPdf(string month)
        {
            var model = GetReportViewModel(month);
            var document = new ExpenseReportPdf(model);
            var pdfBytes = document.GeneratePdf();

            string fileName = $"ExpenseReport_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        private ReportViewModel GetReportViewModel(string month)
        {
            var model = new ReportViewModel
            {
                CategorySummaries = new List<ExpenseCategorySummary>()
            };

            if (!string.IsNullOrEmpty(month))
            {
                if (DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime selectedMonth))
                {
                    var filtered = _context.Expenses
                        .Where(e => e.Date.Month == selectedMonth.Month && e.Date.Year == selectedMonth.Year)
                        .ToList();

                    model.SelectedMonth = selectedMonth.ToString("MMMM yyyy");
                    model.TotalExpenses = filtered.Sum(e => e.Amount);
                    model.CategorySummaries = filtered
                        .GroupBy(e => e.Category)
                        .Select(g => new ExpenseCategorySummary
                        {
                            Category = g.Key,
                            TotalAmount = g.Sum(e => e.Amount)
                        })
                        .ToList();

                    // Fetch budget for this month
                    var budget = _context.Budgets
                        .FirstOrDefault(b => b.Month == model.SelectedMonth && b.Type == "Monthly");

                    if (budget != null)
                    {
                        model.BudgetLimit = budget.Amount;
                    }
                }
            }

            return model;
        }

    }
}


