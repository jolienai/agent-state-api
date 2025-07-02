namespace AgentState.Api.Endpoints;

// centralized registration
internal static class EndpointMappings
{ 
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapCallCenterEventEndpoints();
        // Add more endpoint groups here...
    }
}