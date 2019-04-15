using BankAccounts.Api.Models;
using GraphQL.DataLoader;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api.GraphTypes
{
    public class DbEntryType : ObjectGraphType<Entry>
    {
        public DbEntryType()
        {
            Field(e => e.Id);
            Field(e => e.AccountHolderName);
            Field(e => e.EmployeeName);
            Field(e => e.BankName);
            Field(e => e.BranchName);
            Field(e => e.AccountType);
            Field(e => e.AccountNumber);
            Field(e => e.EmployeeNumber);
            Field(e => e.LastUpdate);
        }
    }
}
