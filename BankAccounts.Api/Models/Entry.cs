using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api.Models
{
    /// <summary>
    /// Employee's bank account entry
    /// </summary>
    /// <remarks>Denormalized to match the view model for simplicity</remarks>
    public class Entry
    {
        public int Id { get; set; }

        public string AccountHolderName { get; set; }

        public string EmployeeName { get; set; }

        public string BankName { get; set; }

        public string BranchName { get; set; }

        public int AccountType { get; set; }

        public long AccountNumber { get; set; }

        public string EmployeeNumber { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
