using AgentState.Application;
using AgentState.Infrastructure;
using Serilog;

namespace AgentState.Api;

public static class Startup
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddHealthChecks()
            .AddNpgSql(connectionString!);
    }

    public static void GracefullyShutdown(this WebApplication app)
    {
        app.Lifetime.ApplicationStopped.Register(() =>
        {
            Task.Run(async () => await Shutdown.OnShutdownAsync());
        });
    }

    public static void AddHeathCheck(this WebApplication app)
    {
        app.MapHealthChecks("/health");
    }
    
    public static void ConfigureMiddleware(this WebApplication app)
    {
        // Add middleware here if needed (e.g., Swagger, CORS, etc.)
        
        // Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
    }

    public static void AddSerilog(this IHostBuilder builder)
    {
        // Attach Serilog to ASP.NET Core and read from configuration
        builder.UseSerilog((ctx, services, config) => config
            .ReadFrom.Configuration(ctx.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
        );
    }
}