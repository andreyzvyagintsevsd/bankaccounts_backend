using BankAccounts.Api.GraphTypes;
using GraphQL;
using GraphQL.Types;
using System;

namespace BankAccounts.Api
{
    public class BankAccountsSchema : Schema
    {
        public BankAccountsSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<BankAccountsQuery>();
            Mutation = resolver.Resolve<BankAccountsMutation>();
        }
    }
}
