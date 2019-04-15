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
using Microsoft.Extensions.Logging;

namespace BankAccounts.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<IStorage, EfBasedEntryStorage>()
                .AddDbContext<BankAccountsDbContext>(opt =>
                {
                    var cs = _configuration.GetConnectionString("DefaultConnection");

                    // We'll use in-memory db if no sql db is specified
                    if (string.IsNullOrEmpty(cs))
                    {
                        _logger.LogWarning($"Using in-memory DB because no connection string is specified");

                        opt.UseInMemoryDatabase("BankAccountsDb");
                    }
                    else
                    {
                        _logger.LogInformation($"Using SQL db");

                        opt.UseSqlServer(cs);
                    }
                });

            // Generic GraphQL services
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>(); // TODO Remove the unused dataloader, perhaps?

            // Application-specific GraphQL services
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
                _logger.LogTrace($"Creating/migrating the database...");
                scope.ServiceProvider.GetService<BankAccountsDbContext>().Database.Migrate();
            }

            app.UseMiddleware<GraphQLMiddleware>();
        }
    }
}
