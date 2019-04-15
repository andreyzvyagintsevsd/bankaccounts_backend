using BankAccounts.Api.Contracts;
using BankAccounts.Api.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BankAccounts.Api.Persistence
{
    public class EfBasedEntryStorage : IStorage
    {
        private readonly BankAccountsDbContext _dbContext;

        public EfBasedEntryStorage(BankAccountsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task FakeSome(int howMany, CancellationToken token)
        {
            var faker = new Faker<Entry>()
                .RuleFor(e => e.AccountHolderName, e => e.Name.FullName())
                .RuleFor(e => e.EmployeeName, e => e.Name.Prefix() + e.Name.FullName())
                .RuleFor(e => e.BankName, e => e.Company.CompanyName())
                .RuleFor(e => e.BranchName, e => "0")
                .RuleFor(e => e.AccountType, e => e.Random.Int(0, 1))
                .RuleFor(e => e.AccountNumber, e => e.Random.Int())
                .RuleFor(e => e.EmployeeNumber, e => e.Random.Int().ToString());

            foreach (var item in faker.GenerateForever().Take(howMany))
            {
                _dbContext.Attach(item);
            }

            await _dbContext.SaveChangesAsync(token);
        }

        public async Task<IEnumerable<Entry>> FindByAccountNumber(long accountNumberMin, long accountNumberMax, CancellationToken token)
        {
            return await _dbContext.Set<Entry>()
                .Where(e =>
                    e.AccountNumber >= accountNumberMin &&
                    e.AccountNumber <= accountNumberMax)
                .ToArrayAsync(token);
        }

        public async Task<IEnumerable<string>> GetBankNames(CancellationToken token)
        {
            return await _dbContext.Set<Entry>()
                .Select(e => e.BankName)
                .Distinct()
                .ToArrayAsync(token);
        }

        public async Task<IEnumerable<string>> GetBranchNames(string bankName, CancellationToken token)
        {
            return await _dbContext.Set<Entry>()
                .Where(e => e.BankName == bankName)
                .Select(e => e.BranchName)
                .Distinct()
                .ToArrayAsync(token);
        }

        public async Task<Entry> GetById(int id, CancellationToken token)
        {
            return await _dbContext.FindAsync<Entry>(id);
        }

        public async Task<IEnumerable<Entry>> List(CancellationToken token)
        {
            return await _dbContext.Set<Entry>().ToArrayAsync(token);
        }

        public async Task Remove(int id, CancellationToken token)
        {
            _dbContext.Remove(await _dbContext.FindAsync<Entry>(id));
            await _dbContext.SaveChangesAsync(token);
        }

        public async Task<Entry> Update(Entry entry, CancellationToken token)
        {
            var add = entry.Id == 0;
            var efEntry = _dbContext.Attach(entry);
            efEntry.State = add ? EntityState.Added : EntityState.Modified;
            entry.LastUpdate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(token);
            return entry;
        }
    }
}
