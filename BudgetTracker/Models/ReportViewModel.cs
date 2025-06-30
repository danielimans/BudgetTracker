namespace ExpenseTracker.Models
{
    public class ExpenseCategorySummary
    {
        public string Category { get; set; }
        public decimal TotalAmount { get; set; }

        // Optional for future use:
        public decimal? CategoryBudgetLimit { get; set; }
        public bool IsCategoryOverBudget => CategoryBudgetLimit.HasValue && TotalAmount > CategoryBudgetLimit.Value;
    }

    public class ReportViewModel
    {
        public string SelectedMonth { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<ExpenseCategorySummary> CategorySummaries { get; set; }

        // PROPERTIES FOR ALERTS
        public decimal? BudgetLimit { get; set; }  // Overall monthly budget (if available)
        public bool IsOverBudget => BudgetLimit.HasValue && TotalExpenses > BudgetLimit.Value;
        public decimal? RemainingBudget => BudgetLimit.HasValue ? BudgetLimit.Value - TotalExpenses : null;
    }
}


