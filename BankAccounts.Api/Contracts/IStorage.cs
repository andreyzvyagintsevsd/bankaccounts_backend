using BankAccounts.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BankAccounts.Api.Contracts
{
    /// <summary>
    /// Storage abstraction / repository for bank account entries
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Lists all bank account entries
        /// </summary>
        Task<IEnumerable<Entry>> List(CancellationToken token);

        /// <summary>
        /// Get specific bank account entry by its id
        /// </summary>
        Task<Entry> GetById(int id, CancellationToken token);

        /// <summary>
        /// Searches the entries that match a range of account numbers
        /// </summary>
        /// <param name="accountNumberMin">The account number to start matching from</param>
        /// <param name="accountNumberMax">The account number to end matching on</param>
        Task<IEnumerable<Entry>> FindByAccountNumber(long accountNumberMin, long accountNumberMax, CancellationToken token);

        /// <summary>
        /// Updates the entry (use id of 0 to add)
        /// </summary>
        /// <returns></returns>
        Task<Entry> AddOrUpdate(Entry entry, CancellationToken token);

        /// <summary>
        /// Gets all bank names in use
        /// </summary>
        Task<IEnumerable<string>> GetBankNames(CancellationToken token);

        /// <summary>
        /// Gets all branch names of the specified bank
        /// </summary>
        Task<IEnumerable<string>> GetBranchNames(string bankName, CancellationToken token);

        /// <summary>
        /// Generates some fake entries to try the application on
        /// </summary>
        Task FakeSome(int howMany, CancellationToken token);

        /// <summary>
        /// Removes an entry by its id
        /// </summary>
        /// <returns></returns>
        Task Remove(int id, CancellationToken token);
    }
}
