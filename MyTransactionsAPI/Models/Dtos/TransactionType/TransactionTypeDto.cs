namespace MyTransactionsAPI.Models.Dtos.TransactionType
{
    public class TransactionTypeDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}
