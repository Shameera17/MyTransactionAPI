using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyTransactionsAPI.Models.Domains;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTransactionsAPI.Models.Dtos.Transaction
{
    public class UpdateTransactionRequest
    {
       
        public required string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public Guid? TransactionTypeId { get; set; }
    }
}
