using BankAccounts.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BankAccounts.Test
{
    public class EfStorageTests
    {
        public EfStorageTests()
        {
            using (var db = GetInMemoryDb())
            {
                db.Database.EnsureDeleted();
            }
        }

        private BankAccountsDbContext GetInMemoryDb(string dbName = default)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankAccountsDbContext>();
            optionsBuilder.UseInMemoryDatabase(dbName ?? "somedb");
            return new BankAccountsDbContext(optionsBuilder.Options);
        }

        [Fact]
        public async Task FakesGetCreated()
        {
            var token = CancellationToken.None;

            using (var db = GetInMemoryDb())
            {
                var storage = new EfBasedEntryStorage(db);
                await storage.FakeSome(10, token);
                Assert.Equal(10, (await storage.List(token)).Count());
                await db.Database.EnsureDeletedAsync(token);
            }
        }

        [Fact]
        public async Task ExistingEntriesCanBeRemoved()
        {
            var token = CancellationToken.None;

            using (var db = GetInMemoryDb())
            {
                var storage = new EfBasedEntryStorage(db);
                await storage.FakeSome(10, token);
                Assert.Equal(10, (await storage.List(token)).Count());
                foreach (var entry in await storage.List(token))
                {
                    await storage.Remove(entry.Id, token);
                }
                Assert.Empty(await storage.List(token));
                await db.Database.EnsureDeletedAsync(token);
            }
        }

        [Fact]
        public async Task SearchingByAccountNumber()
        {
            var token = CancellationToken.None;

            using (var db = GetInMemoryDb())
            {
                var storage = new EfBasedEntryStorage(db);
                await storage.FakeSome(10, token);
                var all = await storage.List(token);
                var min = all.Min(e => e.AccountNumber);
                var max = all.Max(e => e.AccountNumber);
                var exclusive = await storage.FindByAccountNumber(min + 1, max - 1, token);
                Assert.Equal(8, exclusive.Count());
                var inclusive = await storage.FindByAccountNumber(min, max, token);
                Assert.Equal(10, inclusive.Count());
                var inverse = await storage.FindByAccountNumber(max, min, token);
                Assert.Empty(inverse);
                var specificMin = await storage.FindByAccountNumber(min, min, token);
                Assert.Single(specificMin);
                var specificMax = await storage.FindByAccountNumber(max, max, token);
                Assert.Single(specificMax);
                await db.Database.EnsureDeletedAsync(token);
            }
        }

        [Fact]
        public async Task GetByIdWorks()
        {
            var token = CancellationToken.None;

            using (var db = GetInMemoryDb())
            {
                var storage = new EfBasedEntryStorage(db);
                await storage.FakeSome(10, token);
                foreach (var entry in await storage.List(token))
                {
                    Assert.NotEqual(0, entry.Id);
                    Assert.NotNull(await storage.GetById(entry.Id, token));
                }
                await db.Database.EnsureDeletedAsync(token);
            }
        }

        [Fact]
        public async Task UpdateWorks()
        {
            var token = CancellationToken.None;

            using (var db = GetInMemoryDb())
            {
                var storage = new EfBasedEntryStorage(db);
                Assert.Empty(await storage.List(token));
                
                var entry = await storage.Update(new Api.Models.Entry
                {
                    Id = 0,
                    AccountHolderName = string.Empty,
                    AccountNumber = default,
                    AccountType = default,
                    BankName = string.Empty,
                    BranchName = string.Empty,
                    EmployeeName = string.Empty,
                    EmployeeNumber = string.Empty
                }, token);
                Assert.NotEmpty(await storage.List(token));

                entry.BankName = "bank";
                await storage.Update(entry, token);

                Assert.Equal("bank", (await storage.List(token)).Single().BankName);

                await db.Database.EnsureDeletedAsync(token);
            }
        }
    }
}
