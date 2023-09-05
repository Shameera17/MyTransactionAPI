using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTransactionsAPI.Models.Dtos.Transaction
{
    public class CreateTransactionRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        public Guid UserId { get; set; }

        public Guid TransactionTypeId { get; set; }
    }
}
