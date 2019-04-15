using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAccounts.Api.Contracts;
using BankAccounts.Api.GraphTypes;
using BankAccounts.Api.Persistence;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankAccounts.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<IStorage, EfBasedEntryStorage>()
                .AddDbContext<BankAccountsDbContext>(opt =>
                {
                    opt.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
                });

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();

            services.AddScoped<ISchema, BankAccountsSchema>();
            services.AddScoped<BankAccountsQuery>();
            services.AddScoped<BankAccountsMutation>();
            services.AddScoped<DbEntryType>();
            services.AddScoped<DbEntryInputType>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<BankAccountsDbContext>().Database.Migrate();
            }

            app.UseMiddleware<GraphQLMiddleware>();
        }
    }
}
