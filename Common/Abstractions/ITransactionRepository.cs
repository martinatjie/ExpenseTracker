using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Abstractions
{
    public interface ITransactionRepository
    {
        // Create
        Task<Transaction> AddAsync(Transaction transaction);

        // Read one
        Task<Transaction?> GetByIdAsync(Guid id);

        // Read many by predicate
        Task<IEnumerable<Transaction>> GetManyAsync(Func<Transaction, bool> predicate);

        // Read all
        Task<IEnumerable<Transaction>> GetAllAsync();

        // Update one
        Task<bool> UpdateAsync(Transaction transaction);

        // Update all
        Task<int> UpdateManyAsync(IEnumerable<Transaction> transactions);

        // Delete one
        Task<bool> DeleteAsync(Guid id);

        // Delete all
        Task<int> DeleteAllAsync();
    }
}
