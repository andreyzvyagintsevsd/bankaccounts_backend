# Prerequisites

This backend requires .NET Core Runtime 2.2 to run.

# Testing

Run `dotnet test` in the solution folder and observe the results in the console.

# Running

If you have an accessible SQL Server instance at `.\SQLEXPRESS`, please run:

`SET ASPNETCORE_ENVIRONMENT=Production`
`dotnet run -p ./BankAccounts.Api/`

Then use the `/api/graphql` endpoint according to its capabilities.

# Configuration

The easiest way to customize the database connection string is to edit the corresponding option in the appsettings.Production.json configuration file.

# Implementation notes

The scope of this back-end is to provide a simple GraphQL-enabled storage for the test task front-end. The database model is intentionally flat even though it could be normalized to fit additional requirements such as editing bank names in one go. The persistence layer is abstracted away behind `IStorage` and is contained entirely inside the ./Persistence namespace, which under normal circumstances could be isolated within a separate assembly. Given the overall simplicity of the project, the only business logic to undergo meaningful unit-testing is the EF-based storage implementation.