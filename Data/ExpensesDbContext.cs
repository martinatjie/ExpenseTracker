using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ExpensesDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; } = default!;

        public ExpensesDbContext(DbContextOptions<ExpensesDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Id)
                .HasDefaultValueSql("gen_random_uuid()"); // PostgreSQL: auto-generate GUID
        }
    }
}
