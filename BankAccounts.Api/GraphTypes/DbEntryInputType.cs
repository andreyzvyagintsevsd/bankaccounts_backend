using BankAccounts.Api.Models;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api.GraphTypes
{
    public class DbEntryInputType : InputObjectGraphType<Entry>
    {
        public DbEntryInputType()
        {
            Field(e => e.Id);
            Field(e => e.AccountHolderName);
            Field(e => e.EmployeeName);
            Field(e => e.BankName);
            Field(e => e.BranchName);
            Field(e => e.AccountType);
            Field(e => e.AccountNumber);
            Field(e => e.EmployeeNumber);
        }
    }
}
