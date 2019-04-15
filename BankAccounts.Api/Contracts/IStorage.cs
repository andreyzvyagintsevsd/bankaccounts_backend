using BankAccounts.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BankAccounts.Api.Contracts
{
    public interface IStorage
    {
        Task<IEnumerable<Entry>> List(CancellationToken token);

        Task<Entry> GetById(int id, CancellationToken token);

        Task<IEnumerable<Entry>> FindByAccountNumber(long accountNumberMin, long accountNumberMax, CancellationToken token);

        Task<Entry> Update(Entry entry, CancellationToken token);

        Task<IEnumerable<string>> GetBankNames(CancellationToken token);

        Task<IEnumerable<string>> GetBranchNames(string bankName, CancellationToken token);

        Task FakeSome(int howMany, CancellationToken token);

        Task Remove(int id, CancellationToken token);
    }
}
