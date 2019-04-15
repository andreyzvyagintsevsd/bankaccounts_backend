using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api.Persistence
{
    public class BankAccountsDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BankAccountsDbContext>
    {
        public BankAccountsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankAccountsDbContext>();

            optionsBuilder.UseSqlServer("dummy_design_time_connection_string");

            return new BankAccountsDbContext(optionsBuilder.Options);
        }

    }
}