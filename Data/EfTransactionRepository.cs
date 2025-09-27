using Common.Abstractions;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class EfTransactionRepository : ITransactionRepository
    {
        private readonly ExpensesDbContext _context;
        public EfTransactionRepository(ExpensesDbContext context) => _context = context;

        public Task<Transaction> AddAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public Task<Transaction?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transaction>> GetManyAsync(Func<Transaction, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateManyAsync(IEnumerable<Transaction> transactions)
        {
            throw new NotImplementedException();
        }
    }
}
