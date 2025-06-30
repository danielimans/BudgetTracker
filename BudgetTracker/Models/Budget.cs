using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public string Type { get; set; } 
    }
}

