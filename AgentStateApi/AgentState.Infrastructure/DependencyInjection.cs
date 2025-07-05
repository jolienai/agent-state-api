using AgentState.Application;
using AgentState.Application.Features.CallCenter;
using AgentState.Application.Repositories;
using AgentState.Application.Shared;
using AgentState.Infrastructure.Data;
using AgentState.Infrastructure.Migrations;
using AgentState.Infrastructure.Repositories;
using AgentState.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentState.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IRequestHandler<CallCenterEventCommand, Result<bool>>, CallCenterEventHandler>();
        services.AddDbContext(configuration);
        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Run raw SQL migrations
        DbMigrations.Execute(configuration);
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));
        
        services.AddScoped<ICallCenterRepository, CallCenterRepository>();
        return services;
    }
}