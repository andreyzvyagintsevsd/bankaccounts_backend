using BankAccounts.Api.Contracts;
using BankAccounts.Api.Models;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api.GraphTypes
{
    public class BankAccountsQuery : ObjectGraphType
    {
        private readonly IStorage _storage;

        public BankAccountsQuery(IStorage storage)
        {
            _storage = storage;

            Field<ListGraphType<DbEntryType>, IEnumerable<Entry>>()
                .Name("Entries")
                .Argument<IntGraphType>("minAccount", string.Empty)
                .Argument<IntGraphType>("maxAccount", string.Empty)
                .ResolveAsync(async ctx =>
                {
                    var minAcc = ctx.GetArgument<int?>("minAccount");
                    var maxAcc = ctx.GetArgument<int?>("maxAccount");
                    return minAcc.HasValue || maxAcc.HasValue ?
                        await _storage.FindByAccountNumber(minAcc ?? 0, maxAcc ?? int.MaxValue, ctx.CancellationToken) :
                        await _storage.List(ctx.CancellationToken);
                });

            Field<DbEntryType, Entry>()
                .Name("Entry")
                .Argument<IntGraphType>("id", string.Empty)
                .ResolveAsync(async ctx =>
                {
                    return await _storage.GetById(ctx.GetArgument<int>("id"), ctx.CancellationToken);
                });

            Field<ListGraphType<StringGraphType>, IEnumerable<string>>()
                .Name("Banks")
                .ResolveAsync(async ctx =>
                {
                    return await _storage.GetBankNames(ctx.CancellationToken);
                });

            Field<ListGraphType<StringGraphType>, IEnumerable<string>>()
                .Name("Branches")
                .Argument<StringGraphType>("bankName", "The bank to get branches from")
                .ResolveAsync(async ctx =>
                {
                    return await _storage.GetBranchNames(ctx.GetArgument<string>("bankName"), ctx.CancellationToken);
                });
        }
    }
}
