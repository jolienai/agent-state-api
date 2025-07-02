namespace AgentState.Api;

internal static class Shutdown
{
    public static async Task OnShutdownAsync()
    {
        Console.WriteLine("Application is shutting down gracefully...");
        
        // do things to gracefully stop the application
        await Task.Delay(2000);
    }
}