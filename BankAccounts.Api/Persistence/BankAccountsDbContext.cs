using BankAccounts.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api.Persistence
{
    public class BankAccountsDbContext : DbContext
    {
        public BankAccountsDbContext(DbContextOptions<BankAccountsDbContext> options)
            : base(options)
        {

        }

        public DbSet<Entry> Entries { get; set; }
    }
}
