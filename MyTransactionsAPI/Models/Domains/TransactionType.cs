namespace MyTransactionsAPI.Models.Domains
{
    public class TransactionType
    {
        public  Guid Id { get; set; }
        public required string Name { get; set; } 
        public required string Code { get; set; }

        // 1 type can be refferred to many transactions
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
