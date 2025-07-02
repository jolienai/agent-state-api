using AgentState.Api.Extensions;
using AgentState.Api.Models;
using AgentState.Api.Models.Maps;
using AgentState.Application;

namespace AgentState.Api.Endpoints;

internal static class CallCenterEventEndpoints
{
    public static void MapCallCenterEventEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/events/call-center", async (
            CallCenterEventRequest request,
            IMediator mediator, CancellationToken ct) =>
        {
            var command = request.Map();
            var response = await mediator.SendAsync(command, ct);
            return response.ToHttpResult(StatusCodes.Status202Accepted);
        });
    }
}