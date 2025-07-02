using AgentState.Application.Features.CallCenter;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AgentState.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CallCenterEventCommand>, CallCenterEventCommandValidator>();
        return services;
    }
}