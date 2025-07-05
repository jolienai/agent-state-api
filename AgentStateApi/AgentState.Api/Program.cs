using AgentState.Api;
using AgentState.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = BuildApp(builder);
app.Run();

public partial class Program  // 👈 expose this for WebApplicationFactory
{
    public static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        builder.Host.AddSerilog();
        builder.Services.ConfigureServices(builder.Configuration);

        var app = builder.Build();
        app.GracefullyShutdown();
        app.AddHeathCheck();
        app.ConfigureMiddleware();
        app.MapApiEndpoints();

        return app;
    }
}