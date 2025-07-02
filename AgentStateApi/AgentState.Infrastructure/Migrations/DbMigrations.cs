using DbUp;
using Microsoft.Extensions.Configuration;

namespace AgentState.Infrastructure.Migrations;

public static class DbMigrations
{
    public static void Execute(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsFromFileSystem(
                Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts"))
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            throw new Exception("Migration failed", result.Error);
        }

        Console.WriteLine("✅ Database migration successful.");
    }
}