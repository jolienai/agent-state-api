using Testcontainers.PostgreSql;

// ReSharper disable ClassNeverInstantiated.Global

namespace AgentState.Api.Tests.Fixtures;

public class TestContainerFixture : IAsyncLifetime
{
    public readonly PostgreSqlContainer Container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}