using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }
}

