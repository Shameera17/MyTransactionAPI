using Microsoft.EntityFrameworkCore;
using MyTransactionsAPI.Models.Domains;
using System.Reflection.Metadata;

namespace MyTransactionsAPI.Data
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions dbContextOptions): base(dbContextOptions) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(u => u.User)
                .HasForeignKey(transaction => transaction.UserId)
                .IsRequired();

            
        }
    }
}
