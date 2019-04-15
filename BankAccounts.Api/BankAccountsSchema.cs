using BankAccounts.Api.GraphTypes;
using GraphQL;
using GraphQL.Types;
using System;

namespace BankAccounts.Api
{
    /// <summary>
    /// The root GQL schema for this app
    /// </summary>
    public class BankAccountsSchema : Schema
    {
        public BankAccountsSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<BankAccountsQuery>();
            Mutation = resolver.Resolve<BankAccountsMutation>();
        }
    }
}
