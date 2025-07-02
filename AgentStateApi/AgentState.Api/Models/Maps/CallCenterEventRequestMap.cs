using AgentState.Application;
using AgentState.Application.Features.CallCenter;

namespace AgentState.Api.Models.Maps;

internal static class CallCenterEventRequestMap
{
    public static CallCenterEventCommand Map(this CallCenterEventRequest request)
    {
        return new CallCenterEventCommand(
            request.AgentId,
            request.AgentName,
            request.TimestampUtc,
            request.Action,
            request.QueueIds
        );
    }
}