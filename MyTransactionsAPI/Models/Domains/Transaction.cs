using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTransactionsAPI.Models.Domains
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}

        // Define a property for the decimal amount
        // Use the [Column(TypeName = "decimal(precision, scale)")] data annotation
        // to specify precision and scale for your database column
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        // 1 = active
        // 2 = inactive
        public int Status { get; set; }

        // Other properties..
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid TransactionTypeId { get; set; }
        public required TransactionType TransactionType { get; set; }
    }
}
