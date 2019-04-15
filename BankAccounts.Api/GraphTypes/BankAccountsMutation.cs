using BankAccounts.Api.Contracts;
using BankAccounts.Api.Models;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api.GraphTypes
{
    /// <summary>
    /// Defines GQL mutations to manipulate bank account entries
    /// </summary>
    public class BankAccountsMutation : ObjectGraphType
    {
        private readonly IStorage _storage;

        public BankAccountsMutation(IStorage storage)
        {
            _storage = storage;

            Field<DbEntryType, Entry>()
                .Name("addOrUpdateEntry")
                .Argument<DbEntryInputType>("entry", string.Empty)
                .ResolveAsync(async ctx =>
                {
                    return await _storage.AddOrUpdate(ctx.GetArgument<Entry>("entry"), ctx.CancellationToken);
                });

            Field<IntGraphType, int>()
                .Name("remove")
                .Argument<IntGraphType>("id", string.Empty)
                .ResolveAsync(async ctx =>
                {
                    await _storage.FakeSome(ctx.GetArgument<int>("id"), ctx.CancellationToken);
                    return default;
                });

            Field<IntGraphType, int>()
                .Name("fake")
                .Argument<IntGraphType>("howMany", string.Empty)
                .ResolveAsync(async ctx =>
                {
                    await _storage.FakeSome(ctx.GetArgument<int>("howMany"), ctx.CancellationToken);
                    return default;
                });
        }
    }
}
