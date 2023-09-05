using System;

namespace MyTransactionsAPI.Models.Domains
{
    public class User
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required byte[] PasswordHash { get; set; }
        public required byte[] PasswordSalt { get; set; }
        public required string Status { get; set; } = string.Empty;

        // Other properties..
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
