using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccounts.Api
{
    /// <summary>
    /// A middleware to process queries
    /// </summary>
    public class GraphQLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDocumentWriter _writer;
        private readonly IDocumentExecuter _executer;

        public GraphQLMiddleware(RequestDelegate next, IDocumentWriter writer, IDocumentExecuter executer)
        {
            _next = next;
            _writer = writer;
            _executer = executer;
        }

        public async Task InvokeAsync(HttpContext httpContext, ISchema schema, IServiceProvider serviceProvider, ILogger<GraphQLMiddleware> logger)
        {
            if (httpContext.Request.Path.StartsWithSegments("/api/graphql") && string.Equals(httpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogTrace($"Processing GQL query...");
                string body;
                using (var streamReader = new StreamReader(httpContext.Request.Body))
                {
                    body = await streamReader.ReadToEndAsync();

                    var request = JsonConvert.DeserializeObject<GraphQLRequest>(body);

                    var result = await _executer.ExecuteAsync(doc =>
                    {
                        doc.Schema = schema;
                        doc.Query = request.Query;
                        doc.Inputs = request.Variables.ToInputs();
                        doc.Listeners.Add(serviceProvider.GetRequiredService<DataLoaderDocumentListener>());
                    }).ConfigureAwait(false);

                    var json = _writer.Write(result);
                    await httpContext.Response.WriteAsync(json);
                }
            }
            else
            {
                // :TRICKY: This effectively intercepts all non-gql requests.
                // This is not acceptable beyond demonstration purposes.
                using (TextWriter tr = new StreamWriter(httpContext.Response.Body))
                {
                    tr.Write("Please use GraphQL API at /api/graphql");
                }
                httpContext.Response.StatusCode = 400;
                
                // disabled intentionally: await _next(httpContext);
            }
        }

        public class GraphQLRequest
        {
            public string Query { get; set; }
            public JObject Variables { get; set; }
        }
    }
}
