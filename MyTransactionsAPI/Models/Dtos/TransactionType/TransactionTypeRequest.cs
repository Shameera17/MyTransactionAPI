﻿namespace MyTransactionsAPI.Models.Dtos.TransactionType
{
    public class TransactionTypeRequest
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}
