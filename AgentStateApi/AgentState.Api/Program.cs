using AgentState.Api;
using AgentState.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
app.GracefullyShutdown();
app.AddHeathCheck();
app.ConfigureMiddleware();
app.MapApiEndpoints();
app.Run();